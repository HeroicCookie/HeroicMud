namespace HeroicMud.GameLogic;

public class Room
{
	public string Name { get; }
	public string Description { get; }
	public Dictionary<string, Room> Exits { get; } = new();

	public Room(string name, string description)
	{
		Name = name;
		Description = description;
	}

	public void Connect(Room target, string direction)
	{
		Exits[direction] = target;
	}
}
