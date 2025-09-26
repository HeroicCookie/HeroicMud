namespace HeroicMud.GameLogic.Content.Rooms;

public class WindstopInnCellar : Room
{
    public WindstopInnCellar() : base(
        id: "windstop_inn_cellar",
        exits: new()
        {
            ["ladder"] = "windstop_inn_common_room"
        }
    )
    { }

    public override string RenderDescription(Player player)
    {
        return """
			The cellar of the Windstop Inn.
			It's dark and musty, with shelves lined with old barrels and crates.
			A faint light filters in from a small window near the ceiling.
			A** ladder**leads back up to the common room.
			""";
    }
}
