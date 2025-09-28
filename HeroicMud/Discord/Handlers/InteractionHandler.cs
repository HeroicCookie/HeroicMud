using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.Discord.AutoCompleteProviders;
using HeroicMud.Discord.Components;
using HeroicMud.Game;
using HeroicMud.Game.Content.NPCs;
using HeroicMud.Game.Content.Rooms;

namespace HeroicMud.Discord.Handlers;

public class InteractionHandler(Game.World world) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("look", "Look around in the current room.")]
    public async Task LookAsync()
    {
        Player? player = await Context.GetVerifiedPlayerAsync(world);
        if (player is null)
            return;

        await RespondAsync(await world.SendPlayerCommand(new LookCommand(player)));
    }

    [SlashCommand("go", "Move in a direction.")]
    public async Task GoAsync(
        [Autocomplete(typeof(DirectionAutoCompleteProvider))]
        [Summary("direction", "The direction to move.")]
        string direction)
    {
        Player? player = await Context.GetVerifiedPlayerAsync(world);
        if (player is null)
            return;

        await RespondAsync(await world.SendPlayerCommand(new GoCommand(player, direction)));
    }

    [SlashCommand("say", "Say something to others in the room.")]
    public async Task SayAsync([Summary("message", "The message to say.")] string message)
    {
        Player? player = await Context.GetVerifiedPlayerAsync(world);
        if (player is null)
            return;

        Room room = world.RoomManager.GetRoom(player.CurrentRoomId);
        if (room is null)
        {
            await RespondAsync("You are in an unknown place…", ephemeral: true);
            return;
        }

        // Your own confirmation (ephemeral so they don’t see a duplicate in their channel)
        await RespondAsync($"You say: {message}", ephemeral: true);

        // Get all other players in the room
        var others = world.GetPlayersInRoom(room.Id)
            .Where(p => p.DiscordId != player.DiscordId);

        foreach (Player other in others)
        {
            if (ulong.TryParse(other.ChannelId, out var channelId))
            {
                if (Context.Client.GetChannel(channelId) is ITextChannel channel)
                {
                    await channel.SendMessageAsync($"**{player.Name}** says: \"*{message}*\"");
                }
            }
        }
    }

    [SlashCommand("talk", "Talk to an NPC in the room.")]
    public async Task TalkAsync(
        [Autocomplete(typeof(NPCAutoCompleteProvider))]
        [Summary("npc", "The NPC to talk to.")]
        string npcName)
    {
        Player? player = await Context.GetVerifiedPlayerAsync(world);
        if (player is null)
            return;

        var room = world.RoomManager.GetRoom(player.CurrentRoomId);
        if (room is null)
        {
            await RespondAsync("You are in an unknown place…", ephemeral: true);
            return;
        }

        NPC? npc = room.GetNPCs(player).FirstOrDefault(n => n.Name.Equals(npcName, StringComparison.OrdinalIgnoreCase));
        DialogueNode? dialogue = npc?.DialogueNode;
        if (dialogue is null)
        {
            await RespondAsync($"There is no '{npcName}' here to talk to.", ephemeral: true);
            return;
        }

        player.CurrentDialogueResponse = dialogue.Next(player, null);
        await ShowDialogueLineAsync(Context.Interaction, player);
    }

    private async Task ShowDialogueLineAsync(IDiscordInteraction interaction, Player player)
    {
        if (player.CurrentDialogueResponse is null)
        {
            await interaction.RespondAsync("You are not in a conversation.", ephemeral: true);
            return;
        }

        string line = $"{player.CurrentDialogueResponse.Text[0]}";

        var builder = new ComponentBuilder();
        if (player.CurrentDialogueResponse.Text.Count > 1)
        {
            player.CurrentDialogueResponse.Text.RemoveAt(0);
            builder.WithButton("Continue", "dialogue:continue", ButtonStyle.Primary);
        }
        else if (player.CurrentDialogueResponse.Options.Count > 0)
        {
            foreach (var option in player.CurrentDialogueResponse.Options)
            {
                builder.WithButton(option, "dialogue:" + option, ButtonStyle.Primary);
            }
        }
        else
        {
            player.CurrentDialogueResponse = null;
        }

        if (interaction is SocketMessageComponent componentInteraction)
        {
            await componentInteraction.UpdateAsync(msg =>
            {
                msg.Content = line;
                msg.Components = builder.Build();
            });
        }
        else
        {
            await interaction.RespondAsync(line, components: builder.Build());
        }
    }


    [ComponentInteraction("dialogue:*", true)]
    public async Task HandleDialogueOptionAsync(string customId)
    {
        Player? player = await Context.GetVerifiedPlayerAsync(world);
        if (player is null)
            return;

        if (player.CurrentDialogueResponse is null)
        {
            await Context.Interaction.RespondAsync("You are not in a conversation.", ephemeral: true);
            return;
        }

        if (customId is not "continue")
        {
            if (!player.CurrentDialogueResponse.Options.Contains(customId))
            {
                await Context.Interaction.RespondAsync("Invalid dialogue option.", ephemeral: true);
                return;
            }

            if (player.CurrentDialogueResponse.Node is null)
            {
                await Context.Interaction.RespondAsync("The conversation has ended.", ephemeral: true);
                return;
            }

            player.CurrentDialogueResponse = player.CurrentDialogueResponse.Node.Next(player, customId);
        }

        await ShowDialogueLineAsync(Context.Interaction, player);
    }

    [SlashCommand("create", "Create a new character.")]
    public async Task CreateAsync()
    {
        bool playerExists = world.PlayerExists(Context.User.Id.ToString());
        if (playerExists is true)
        {
            await RespondAsync("You already have a character.", ephemeral: true);
            return;
        }

        await Context.Interaction.RespondWithModalAsync<CharacterModal>("create_character_modal");
    }

    // Character Modal Submission Handler
    [ModalInteraction("create_character_modal", true)]
    public async Task HandleCharacterModalAsync(CharacterModal modal)
    {
        string name = modal.Name.Trim();
        string description = modal.Description.Trim();

        ITextChannel? channel = await Context.CreatePrivateChannelAsync(name);
        if (channel is null)
        {
            await RespondAsync("Could not create your private channel.", ephemeral: true);
            return;
        }

        SaveResult result = await world.CreatePlayerAsync(
            Context.User.Id.ToString(),
            channel.Id.ToString(),
            name,
            description
        );

        switch (result)
        {
            case SaveResult.Created:
                await RespondAsync($"Character '{name}' created! Your private channel is <#{channel.Id}>.", ephemeral: true);
                await channel.SendMessageAsync($"Welcome, {name}! This is your private channel. Use /look to see your surroundings.");
                break;

            case SaveResult.Error:
            default:
                await RespondAsync("Failed to create character. Try again later.", ephemeral: true);
                break;
        }
    }
}
