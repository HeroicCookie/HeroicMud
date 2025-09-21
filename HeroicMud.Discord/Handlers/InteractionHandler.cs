using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.Discord.AutoCompleteProviders;
using HeroicMud.Discord.Components;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Data.NPCs;
using HeroicMud.GameLogic.Data.Rooms;
using HeroicMud.GameLogic.Enums;

namespace HeroicMud.Discord.Handlers;

public class InteractionHandler(MudGame game, RoomManager roomManager) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("look", "Look around in the current room.")]
    public async Task LookAsync()
    {
        Player? player = await DiscordHelpers.GetVerifiedPlayerAsync(Context, game);
        if (player == null) return;

        string discordId = Context.User.Id.ToString();

        string output = game.HandlePlayerCommand(discordId, GameCommand.Look);
        await RespondAsync(output);
    }

    [SlashCommand("go", "Move in a direction.")]
    public async Task GoAsync(
        [Autocomplete(typeof(DirectionAutoCompleteProvider))]
        [Summary("direction", "The direction to move.")] string direction)
    {
        Player? player = await DiscordHelpers.GetVerifiedPlayerAsync(Context, game);
        if (player == null) return;

        string discordId = Context.User.Id.ToString();

        string output = game.HandlePlayerCommand(discordId, GameCommand.Go, direction);
        await RespondAsync(output);
    }

    [SlashCommand("say", "Say something to others in the room.")]
    public async Task SayAsync([Summary("message", "The message to say.")] string message)
    {
        Player? player = await DiscordHelpers.GetVerifiedPlayerAsync(Context, game);
        if (player is null) return;

        var room = roomManager.GetRoom(player.CurrentRoomId);
        if (room == null)
        {
            await RespondAsync("You are in an unknown place…", ephemeral: true);
            return;
        }

        // Your own confirmation (ephemeral so they don’t see a duplicate in their channel)
        await RespondAsync($"You say: {message}", ephemeral: true);

        // Get all other players in the room
        var others = game.GetPlayersInRoom(room.Id)
            .Where(p => p.DiscordId != player.DiscordId);

        foreach (var other in others)
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
        [Summary("npc", "The NPC to talk to.")] string npcName)
    {
        Player? player = await DiscordHelpers.GetVerifiedPlayerAsync(Context, game);
        if (player is null) return;

        var room = roomManager.GetRoom(player.CurrentRoomId);
        if (room == null)
        {
            await RespondAsync("You are in an unknown place…", ephemeral: true);
            return;
        }

        DialogueNode? dialogue = room.Dialogues.FirstOrDefault(n => n.Name.Equals(npcName, StringComparison.OrdinalIgnoreCase));
        if (dialogue == null)
        {
            await RespondAsync($"There is no '{npcName}' here to talk to.", ephemeral: true);
            return;
        }

        var response = dialogue.Next(player, null);
        var components = BuildDialogueOptions(response.Options);
        player.CurrentDialogueNode = response.Node;
        await RespondAsync(dialogue.Response(player)[0], components: components);
    }

	[ComponentInteraction("*", true)]
	public async Task HandleDialogueOptionAsync(string customId)
	{
		Player? player = await DiscordHelpers.GetVerifiedPlayerAsync(Context, game);
		if (player == null) return;

		var dialogue = player.CurrentDialogueNode;
		if (dialogue == null)
		{
			await Context.Interaction.RespondAsync("You are not in a conversation.", ephemeral: true);
			return;
		}

		DialogueResponse response = dialogue.Next(player, customId);
		player.CurrentDialogueNode = response.Node;

        if (player.CurrentDialogueNode == null)
        {
            await Context.Interaction.RespondAsync("The conversation has ended.", ephemeral: true);
            return;
		}

		var interaction = Context.Interaction as SocketMessageComponent;
        if (interaction == null)
        {
            await Context.Interaction.RespondAsync("Interaction error.", ephemeral: true);
            return;
		}

		if (response.Options.Count == 0)
		{
			await interaction.UpdateAsync(msg =>
			{
				msg.Content = player.CurrentDialogueNode.Response(player)[0];
                msg.Components = null;
			});
			return;
		}

		var components = BuildDialogueOptions(response.Options);
		await interaction.UpdateAsync(msg =>
		{
			msg.Content = player.CurrentDialogueNode.Response(player)[0];
			msg.Components = components;
		});
	}

	// Build interaction response for dialogue options
	public MessageComponent BuildDialogueOptions(List<string> options)
    {
        var builder = new ComponentBuilder();
        foreach (var option in options)
        {
            builder.WithButton(option, option, ButtonStyle.Primary);
        }
        return builder.Build();
    }

    [SlashCommand("create", "Create a new character.")]
    public async Task CreateAsync()
    {
        bool playerExists = game.PlayerExists(Context.User.Id.ToString());
        if (playerExists == true)
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

        ITextChannel? channel = await DiscordHelpers.CreatePrivateChannelAsync(Context, name);
        if (channel == null)
        {
            await RespondAsync("Could not create your private channel.", ephemeral: true);
            return;
        }

        SaveResult result = await game.CreatePlayerAsync(
            Context.User.Id.ToString(),
            channel.Id.ToString(),
            name,
            description,
            'm');

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
