using HeroicMud.GameLogic.Data.Entity;

namespace HeroicMud.GameLogic.Data.NPCs;

public abstract class NPC : RoomEntity
{
	public NPC(string id, string name) : base(id, name) {}

	public abstract DialogueNode DialogueNode { get; }
}