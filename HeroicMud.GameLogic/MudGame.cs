using HeroicMud.GameLogic.Data.Rooms;
using HeroicMud.GameLogic.Enums;
using HeroicMud.GameLogic.PlayerRepository;
using HeroicMud.GameLogic.Room;
using HeroicMud.GameLogic.TickLogic;

namespace HeroicMud.GameLogic;

public class MudGame
{
	private readonly IPlayerRepository _playerRepository;
	private readonly TickManager _tickManager;
	private readonly RoomManager _roomManager;
	private List<Player> players = new();

	public MudGame(IPlayerRepository playerRepository, TickManager tickManager, RoomManager roomManager)
	{
		_playerRepository = playerRepository;
		_tickManager = tickManager;
		_roomManager = roomManager;
	}

	public async Task LoadPlayersAsync()
	{
		players = await _playerRepository.GetAllAsync();
		foreach (var player in players)
		{
			_tickManager.Register(player);
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
		_tickManager.Register(player);
		return await _playerRepository.CreateAsync(player);
	}

	public async Task<SaveResult> SavePlayerAsync(Player player)
	{
		return await _playerRepository.UpdateAsync(player);
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
		IRoom? currentRoom = _roomManager.GetRoom(player.CurrentRoomId);
		if (currentRoom is null)
		{
			return "You are nowhere. This is a bug.";
		}

		return currentRoom.RenderDescription(player);
	}

	private async Task<string> HandleGo(Player player, string direction)
	{
		IRoom? currentRoom = _roomManager.GetRoom(player.CurrentRoomId);
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

			currentRoom = _roomManager.GetRoom(player.CurrentRoomId);
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

