using HeroicMud.GameLogic.TickLogic;

namespace HeroicMud.GameLogic;

public class Player : ITickable
{
    public required string DiscordId { get; set; } = string.Empty;
    public required string ChannelId { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required char Gender { get; set; }
    public required string Description { get; set; } = string.Empty;
	public string CurrentRoomId { get; set; } = string.Empty;

    public int AttackCooldown { get; private set; } = 0;

    public override void OnTick()
    {
        if (AttackCooldown > 0)
        {
            AttackCooldown--;
        }
        else if (AttackCooldown <= 0)
        {
            Console.WriteLine($"{Name} attacks!");
            AttackCooldown = 3;
        }
    }
}
