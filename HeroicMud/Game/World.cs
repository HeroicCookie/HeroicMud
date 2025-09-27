using HeroicMud.Game.Content.Rooms;
using HeroicMud.Game.PlayerRepository;
using HeroicMud.Game.Ticking;

namespace HeroicMud.Game;

public class World(IPlayerRepository playerRepository)
{
    public readonly IPlayerRepository PlayerRepository = playerRepository;
    public readonly RoomManager RoomManager = new();

    private readonly Ticker ticker = new();
    private List<Player> _players = [];

    public async Task Start()
    {
        _players = await PlayerRepository.GetAllAsync();
        foreach (var player in _players)
            ticker.Register(player);
        ticker.Start();
    }

    public async Task Stop()
    {
        await ticker.Stop();
    }

    public async Task<SaveResult> CreatePlayerAsync(string discordId, string channelId, string name, string description)
    {
        Player player = new()
        {
            DiscordId = discordId,
            ChannelId = channelId,
            Name = name,
            Description = description,
            CurrentRoomId = "windstop_inn_common_room"
        };

        _players.Add(player);
        ticker.Register(player);

        return await PlayerRepository.CreateAsync(player);
    }

    public async Task<SaveResult> SavePlayerAsync(Player player) => await PlayerRepository.UpdateAsync(player);

    public string HandlePlayerCommand(string discordId, GameCommand command, params string[] args)
    {
        Player? player = _players.FirstOrDefault(p => p.DiscordId == discordId);
        if (player is null)
            return "You need to create a character first using /create.";

        return command switch
        {
            GameCommand.Look => HandleLook(player),
            GameCommand.Go => HandleGo(player, args.FirstOrDefault() ?? "").Result,
            GameCommand.Say => HandleSay(player, string.Join(" ", args)),
            _ => "Unknown command."
        };
    }

    private string HandleLook(Player player)
    {
        Room? currentRoom = RoomManager.GetRoom(player.CurrentRoomId);
        if (currentRoom is null)
            return "You are nowhere. This is a bug.";

        return currentRoom.RenderDescription(player);
    }

    private async Task<string> HandleGo(Player player, string direction)
    {
        Room? currentRoom = RoomManager.GetRoom(player.CurrentRoomId);
        if (string.IsNullOrWhiteSpace(direction))
            return "Go where?";

        if (currentRoom is null)
            return "You are nowhere. This is a bug.";

        if (currentRoom.GetExits(player).TryGetValue(direction.ToLower(), out string? nextRoomId))
        {
            player.CurrentRoomId = nextRoomId;
            _ = await SavePlayerAsync(player); // TODO: Add some user feedback if this fails
            currentRoom = RoomManager.GetRoom(player.CurrentRoomId);

            return currentRoom.RenderDescription(player);
        }

        return "You can't go that way.";
    }

    private string HandleSay(Player player, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Say what?";

        return $"{player.Name} says: {message}";
    }

    public Player? GetPlayer(string discordId) => _players.FirstOrDefault(p => p.DiscordId == discordId);

    public bool PlayerExists(string discordId) => GetPlayer(discordId) != null;

    public IEnumerable<Player> GetPlayersInRoom(string roomId) => _players.Where(p => p.CurrentRoomId == roomId);
}
