using System.Reflection;

namespace HeroicMud.GameLogic.Data.Rooms;

public class RoomManager
{
    private readonly Room[] _rooms;

    public RoomManager()
    {
		_rooms = [.. Assembly.GetExecutingAssembly().GetTypes()
	    .Where(t => t.Namespace == typeof(Room).Namespace)
	    .Where(t => t.IsSubclassOf(typeof(Room)) && t.GetConstructor(Type.EmptyTypes) is not null)
	    .Select(t => (Room)Activator.CreateInstance(t)!)
    ];
        Console.WriteLine($"Loaded {_rooms.Length} rooms.");
	}

    public Room GetRoom(string roomId)
    {
        return _rooms.FirstOrDefault(r => r.Id == roomId)
           ?? throw new InvalidOperationException($"Room '{roomId}' not found");
    }
}
