using System.Reflection;

namespace HeroicMud.GameLogic.Data.Rooms;

public class RoomManager
{
    private readonly Room[] _rooms;

    public RoomManager()
    {
        _rooms = [.. Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.Namespace == typeof(Room).Namespace)
            .Select(t =>
            {
                if (typeof(Room).IsAssignableFrom(t))
                {
                    object? room = Activator.CreateInstance(t);
                    if (room is not null)
                        return (Room)room;
                    return null;
                }
                return null;
            })
            .OfType<Room>()
        ];
        Console.WriteLine(String.Join(' ', _rooms.Select(r => r.Id)));
    }

    public Room GetRoom(string roomId)
    {
        return _rooms.FirstOrDefault(r => r.Id == roomId)
           ?? throw new InvalidOperationException($"Room '{roomId}' not found");
    }
}
