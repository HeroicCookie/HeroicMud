namespace HeroicMud.Game.Content.Entity;

public abstract class RoomEntity(string id, string name)
{
	public string Id { get; } = id;
	public string Name { get; } = name;

	// Required for all entities to provide a description for /look command
	public abstract string GetDescription(Player player);
}
