namespace HeroicMud.GameLogic.Room;

public interface IRoom
{
	string Id { get; }

	/// <summary>
	/// Key: keyword for the exit (e.g., "north", "south", "up", "down", "inn cellar door")
	/// Value: ID of the room that the exit leads to
	/// </summary>
	Dictionary<string, string> Exits { get; }
	string RenderDescription(Player player);
}
