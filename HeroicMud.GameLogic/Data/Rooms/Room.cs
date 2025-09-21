using HeroicMud.GameLogic.Data.NPCs;

namespace HeroicMud.GameLogic.Data.Rooms;

public abstract class Room(string id, Dictionary<string, string> exits)
{
    public readonly string Id = id;

    /// <summary>
    /// Key: keyword for the exit (e.g., "north", "south", "up", "down", "inn cellar door")
    /// Value: ID of the room that the exit leads to
    /// </summary>
    public readonly Dictionary<string, string> Exits = exits;

    public readonly List<DialogueNode> Dialogues = new();

	public virtual string RenderDescription(Player player) => "Room has no description.";
}
