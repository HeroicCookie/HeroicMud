using HeroicMud.Game.Content.Entity;

namespace HeroicMud.Game.Content.NPCs;

public abstract class NPC(string id, string name) : RoomEntity(id, name)
{
	public abstract DialogueNode DialogueNode { get; }
}