namespace HeroicMud.GameLogic.Data.Rooms;

public interface IRoom
{
    public static IRoom Instance { get; } = null!;

    public string Id { get; }

    /// <summary>
    /// Key: keyword for the exit (e.g., "north", "south", "up", "down", "inn cellar door")
    /// Value: ID of the room that the exit leads to
    /// </summary>
    public Dictionary<string, string> Exits { get; }

    public string RenderDescription(Player player);
}
