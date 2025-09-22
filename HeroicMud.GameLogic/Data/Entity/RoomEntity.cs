namespace HeroicMud.GameLogic.Data.Entity;

public abstract class RoomEntity
{
	public string Id { get; }
	public string Name { get; }

	protected RoomEntity(string id, string name)
	{
		Id = id;
		Name = name;
	}

	// Required for all entities to provide a description for /look command
	public abstract string GetDescription(Player player);
}
