using HeroicMud.GameLogic.Content.Entity;

namespace HeroicMud.GameLogic.Content.NPCs;

public abstract class NPC(string id, string name) : RoomEntity(id, name)
{
	public abstract DialogueNode DialogueNode { get; }
}