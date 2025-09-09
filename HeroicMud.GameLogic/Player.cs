using HeroicMud.GameLogic.Enums;
using HeroicMud.GameLogic.PlayerRepository;

namespace HeroicMud.GameLogic;
public class Player
{
	public required string DiscordId { get; set; } = string.Empty;
	public required string Name { get; set; } = string.Empty;
	public required char Gender { get; set; }

	public Room? CurrentRoom { get; set; }
}
