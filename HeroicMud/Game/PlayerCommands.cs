using HeroicMud.Game.Content.Rooms;

namespace HeroicMud.Game;

public class LookCommand(Player player) : PlayerCommand
{
    public override Task<string> Action(World world)
    {
        Room? currentRoom = world.RoomManager.GetRoom(player.CurrentRoomId);
        if (currentRoom is null)
            return Task.FromResult("You are nowhere. This is a bug.");

        return Task.FromResult(currentRoom.RenderDescription(player));
    }
}

public class GoCommand(Player player, string direction) : PlayerCommand
{
    public override async Task<string> Action(World world)
    {
        Room? currentRoom = world.RoomManager.GetRoom(player.CurrentRoomId);
        if (string.IsNullOrWhiteSpace(direction))
            return "Go where?";

        if (currentRoom is null)
            return "You are nowhere. This is a bug.";

        if (currentRoom.GetExits(player).TryGetValue(direction.ToLower(), out string? nextRoomId))
        {
            player.CurrentRoomId = nextRoomId;
            _ = await world.SavePlayerAsync(player); // TODO: Add some user feedback if this fails
            currentRoom = world.RoomManager.GetRoom(player.CurrentRoomId);

            return currentRoom.RenderDescription(player);
        }

        return "You can't go that way.";
    }
}

public abstract class PlayerCommand
{
    public readonly Task<string> Description;

    private readonly TaskCompletionSource<string> _completion = new();

    public PlayerCommand() => Description = _completion.Task;

    public async Task Do(World world)
    {
        _completion.SetResult(await Action(world));
    }

    public abstract Task<string> Action(World world);
}
