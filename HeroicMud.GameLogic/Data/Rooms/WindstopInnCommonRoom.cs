using HeroicMud.GameLogic.Room;

namespace HeroicMud.GameLogic.Data.Rooms;

public class WindstopInnCommonRoom : IRoom
{
	public string Id => "windstop_inn_common_room";

	public Dictionary<string, string> Exits => new()
	{
		{ "cellar door", "windstop_inn_cellar" }
	};

	public string RenderDescription(Player player)
	{
		return @"The common room of the Windstop Inn.
A **cellar door** leads below.";
	}
}
