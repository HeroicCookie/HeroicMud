using HeroicMud.GameLogic.Room;

namespace HeroicMud.GameLogic.Data.Rooms;

public class WindstopInnCellar : IRoom
{
	public string Id => "windstop_inn_cellar";
	public Dictionary<string, string> Exits => new()
	{
		{ "ladder", "windstop_inn_common_room" }
	};
	public string RenderDescription(Player player)
	{
		return @"The cellar of the Windstop Inn. 
It's dark and musty, with shelves lined with old barrels and crates. 
A faint light filters in from a small window near the ceiling. 
A **ladder** leads back up to the common room.";
	}
}
