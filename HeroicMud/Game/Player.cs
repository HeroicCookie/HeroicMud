using HeroicMud.Game.Content.NPCs;
using HeroicMud.Game.Ticking;

namespace HeroicMud.Game;

public class Player : ITickable
{
    public required string DiscordId { get; set; } = string.Empty;
    public required string ChannelId { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;

    public string CurrentRoomId { get; set; } = string.Empty;
    public DialogueResponse? CurrentDialogueResponse { get; set; } = null;

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
