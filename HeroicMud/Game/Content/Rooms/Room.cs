using HeroicMud.Game.Content.NPCs;

namespace HeroicMud.Game.Content.Rooms;

public abstract class Room(string id, Dictionary<string, string> exits)
{
    public readonly string Id = id;

    /// <summary>
    /// Key: keyword for the exit (e.g., "north", "south", "up", "down", "inn cellar door")
    /// Value: ID of the room that the exit leads to
    /// </summary>
    public readonly Dictionary<string, string> Exits = exits;

    public readonly List<NPC> NPCs = [];

    public virtual string RenderDescription(Player player) => "Room has no description.";

    /// <summary>
    /// Override to provide a custom list of exits based on player information.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual Dictionary<string, string> GetExits(Player player) => Exits;

    /// <summary>
    /// Override to provide a custom list of NPCs based on player information.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public virtual List<NPC> GetNPCs(Player player) => NPCs;
}
