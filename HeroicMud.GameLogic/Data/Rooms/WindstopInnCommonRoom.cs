namespace HeroicMud.GameLogic.Data.Rooms;

public class WindstopInnCommonRoom : Room
{
    public WindstopInnCommonRoom() : base(
        id: "windstop_inn_common_room",
        exits: new()
        {
            ["cellar door"] = "windstop_inn_cellar"
        }
    )
    { }

    public override string RenderDescription(Player player)
    {
        return @"The common room of the Windstop Inn.
A **cellar door** leads below.";
    }
}
