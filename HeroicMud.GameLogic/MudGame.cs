using HeroicMud.GameLogic.Enums;
using HeroicMud.GameLogic.PlayerRepository;
using System.Threading.Tasks;

namespace HeroicMud.GameLogic;

public class MudGame
{
	private readonly IPlayerRepository _playerRepository;

	public MudGame(IPlayerRepository playerRepository)
	{
		_playerRepository = playerRepository;
	}

	public async Task<SaveResult> CreatePlayerAsync(string discordId, string name, char gender)
	{
		Player player = new()
		{
			DiscordId = discordId,
			Name = name,
			Gender = 'm'
		};

		return await _playerRepository.CreateAsync(player);
	}

	public async Task<Player?> GetPlayerAsync(string discordId)
	{
		return await _playerRepository.GetAsync(discordId);
	}

	public async Task<SaveResult> SavePlayerAsync(Player player)
	{
		return await _playerRepository.UpdateAsync(player);
	}

	public async Task<string> HandlePlayerCommand(string discordId, GameCommand command, params string[] args)
	{
		Player? player = await GetPlayerAsync(discordId);

		if (player == null)
		{
			return "You need to create a character first using /create.";
		}

		return command switch
		{
			GameCommand.Look => HandleLook(player),
			GameCommand.Go => HandleGo(player, args.FirstOrDefault() ?? ""),
			GameCommand.Say => HandleSay(player, string.Join(" ", args)),
			_ => "Unknown command."
		};
	}

	private string HandleLook(Player player)
	{
		var room = player.CurrentRoom;
		if (room is null)
		{
			return "You are nowhere. This is a bug.";
		}
			
		var exits = room.Exits.Keys.Any() ? string.Join(", ", room.Exits.Keys) : "none";
		return $"{room.Name}\n{room.Description}\nExits: {exits}";
	}

	private string HandleGo(Player player, string direction)
	{
		if (string.IsNullOrWhiteSpace(direction))
			return "Go where?";

		if (player.CurrentRoom is null)
		{
			return "You are nowhere. This is a bug.";
		}

		if (player.CurrentRoom.Exits.TryGetValue(direction.ToLowerInvariant(), out var nextRoom))
		{
			player.CurrentRoom = nextRoom;
			return HandleLook(player);
		}
		else
		{
			return "You can't go that way.";
		}
	}

	private string HandleSay(Player player, string message)
	{
		if (string.IsNullOrWhiteSpace(message))
			return "Say what?";

		return $"{player.Name} says: {message}";
	}
}

