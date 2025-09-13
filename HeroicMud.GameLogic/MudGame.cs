using HeroicMud.GameLogic.Data.Rooms;
using HeroicMud.GameLogic.Enums;
using HeroicMud.GameLogic.PlayerRepository;
using HeroicMud.GameLogic.TickLogic;

namespace HeroicMud.GameLogic;

public class MudGame(IPlayerRepository playerRepository, TickManager tickManager, RoomManager roomManager)
{
    private List<Player> players = [];

    public async Task LoadPlayersAsync()
    {
        players = await playerRepository.GetAllAsync();
        foreach (var player in players)
        {
            tickManager.Register(player);
        }
    }

    public async Task<SaveResult> CreatePlayerAsync(string discordId, string channelId, string name, char gender)
    {
        Player player = new()
        {
            DiscordId = discordId,
            ChannelId = channelId,
            Name = name,
            Gender = 'm',
            CurrentRoomId = "windstop_inn_common_room"
        };

        players.Add(player);
        tickManager.Register(player);
        return await playerRepository.CreateAsync(player);
    }

    public async Task<SaveResult> SavePlayerAsync(Player player)
    {
        return await playerRepository.UpdateAsync(player);
    }

    public string HandlePlayerCommand(string discordId, GameCommand command, params string[] args)
    {
        Player? player = players.FirstOrDefault(p => p.DiscordId == discordId);

        if (player == null)
        {
            return "You need to create a character first using /create.";
        }

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
        IRoom? currentRoom = roomManager.GetRoom(player.CurrentRoomId);
        if (currentRoom is null)
        {
            return "You are nowhere. This is a bug.";
        }

        return currentRoom.RenderDescription(player);
    }

    private async Task<string> HandleGo(Player player, string direction)
    {
        IRoom? currentRoom = roomManager.GetRoom(player.CurrentRoomId);
        if (string.IsNullOrWhiteSpace(direction))
            return "Go where?";

        if (currentRoom is null)
        {
            return "You are nowhere. This is a bug.";
        }

        if (currentRoom.Exits.TryGetValue(direction.ToLower(), out string? nextRoomId))
        {
            player.CurrentRoomId = nextRoomId;
            _ = await SavePlayerAsync(player); // TODO: Add some user feedback if this fails

            currentRoom = roomManager.GetRoom(player.CurrentRoomId);
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

    public Player? GetPlayer(string discordId)
    {
        Player? player = players.FirstOrDefault(p => p.DiscordId == discordId);
        return player;
    }

    public bool PlayerExists(string discordId) => GetPlayer(discordId) != null;

    public IEnumerable<Player> GetPlayersInRoom(string roomId) => players.Where(p => p.CurrentRoomId == roomId);
}
