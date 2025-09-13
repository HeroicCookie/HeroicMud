using HeroicMud.GameLogic.Data.Rooms;

namespace HeroicMud.GameLogic.Room;

public class RoomManager
{
	private readonly List<IRoom> _rooms;

	public RoomManager()
	{
		_rooms = new List<IRoom>
		{
			new WindstopInnCommonRoom(),
			new WindstopInnCellar()
		};
	}

	public IRoom GetRoom(string roomId)
	{
		return _rooms.FirstOrDefault(r => r.Id == roomId)
		   ?? throw new InvalidOperationException($"Room '{roomId}' not found");
	}
}
