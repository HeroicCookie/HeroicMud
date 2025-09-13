using System.Reflection;

namespace HeroicMud.GameLogic.Data.Rooms;

public class RoomManager
{
    private readonly IRoom[] _rooms;

    public RoomManager()
    {
        _rooms = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.Namespace == typeof(IRoom).Namespace)
            .Select(t =>
            {
                if (!typeof(IRoom).IsAssignableFrom(t))
                    return null;

                FieldInfo? instance = t.GetField("Instance");
                if (instance is null)
                    return null;
                if (!typeof(IRoom).IsAssignableFrom(instance.FieldType))
                    return null;

                return (IRoom?)instance.GetValue(null);
            })
            .OfType<IRoom>()
            .ToArray();
        Console.WriteLine(String.Join(' ', _rooms.Select(r => r.Id)));
    }

    public IRoom GetRoom(string roomId)
    {
        return _rooms.FirstOrDefault(r => r.Id == roomId)
           ?? throw new InvalidOperationException($"Room '{roomId}' not found");
    }
}
