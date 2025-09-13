using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.Discord.AutoCompleteProviders;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Enums;
using HeroicMud.GameLogic.Room;

namespace HeroicMud.Discord;

public class InteractionHandler : InteractionModuleBase<SocketInteractionContext>
{
	private readonly MudGame _game;
	private readonly RoomManager _roomManager;

	public InteractionHandler(MudGame game, RoomManager roomManager)
	{
		_game = game;
		_roomManager = roomManager;
	}

	[SlashCommand("look", "Look around in the current room.")]
	public async Task LookAsync()
	{
		Player? player = await GetVerifiedPlayerAsync(Context);
		if (player == null) return;

		string discordId = Context.User.Id.ToString();

		string output = _game.HandlePlayerCommand(discordId, GameCommand.Look);
		await RespondAsync(output);
	}

	[SlashCommand("go", "Move in a direction.")]
	public async Task GoAsync(
		[Autocomplete(typeof(DirectionAutoCompleteProvider))]
		[Summary("direction", "The direction to move.")] string direction)
	{
		Player? player = await GetVerifiedPlayerAsync(Context);
		if (player == null) return;

		string discordId = Context.User.Id.ToString();

		string output = _game.HandlePlayerCommand(discordId, GameCommand.Go, direction);
		await RespondAsync(output);
	}

	[SlashCommand("say", "Say something to others in the room.")]
	public async Task SayAsync([Summary("message", "The message to say.")] string message)
	{
		Player? player = await GetVerifiedPlayerAsync(Context);
		if (player == null) return;

		var room = _roomManager.GetRoom(player.CurrentRoomId);
		if (room == null)
		{
			await RespondAsync("You are in an unknown place…", ephemeral: true);
			return;
		}

		// Your own confirmation (ephemeral so they don’t see a duplicate in their channel)
		await RespondAsync($"You say: {message}", ephemeral: true);

		// Get all other players in the room
		var others = _game.GetPlayersInRoom(room.Id)
			.Where(p => p.DiscordId != player.DiscordId);

		foreach (var other in others)
		{
			if (ulong.TryParse(other.ChannelId, out var channelId))
			{
				var channel = Context.Client.GetChannel(channelId) as ITextChannel;
				if (channel != null)
				{
					await channel.SendMessageAsync($"**{player.Name}** says: \"*{message}*\"");
				}
			}
		}
	}


	[SlashCommand("create", "Create a new character.")]
	public async Task CreateAsync([Summary("name", "Name thyself.")] string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			await RespondAsync("Name cannot be empty.", ephemeral: true);
			return;
		}

		bool playerExists = _game.PlayerExists(Context.User.Id.ToString());
		if (playerExists == true)
		{
			await RespondAsync("You already have a character.", ephemeral: true);
			return;
		}

		// Create a private text channel for the player
		ITextChannel? channel = await CreatePrivateChannelAsync(Context, name);
		if (channel == null)
		{
			await RespondAsync("Could not create your private channel.", ephemeral: true);
			return;
		}

		SaveResult result = await _game.CreatePlayerAsync(
			Context.User.Id.ToString(),
			channel.Id.ToString(),
			name,
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

	private async Task<ITextChannel?> CreatePrivateChannelAsync(SocketInteractionContext context, string name)
	{
		if (context.Channel is not SocketTextChannel parentChannel)
			return null;

		var guild = parentChannel.Guild;
		var categoryId = parentChannel.CategoryId;

		var overwrites = new List<Overwrite>
	{
		new(guild.EveryoneRole.Id, PermissionTarget.Role,
			new OverwritePermissions(viewChannel: PermValue.Deny)),

		new(context.User.Id, PermissionTarget.User,
			new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow)),

		new(guild.CurrentUser.Id, PermissionTarget.User,
			new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow))
	};

		return await guild.CreateTextChannelAsync(name, props =>
		{
			props.CategoryId = categoryId;
			props.PermissionOverwrites = overwrites;
		});
	}

	private async Task<Player?> GetVerifiedPlayerAsync(IInteractionContext context)
	{
		var player = _game.GetPlayer(context.User.Id.ToString());

		if (player == null)
		{
			await context.Interaction.RespondAsync(
				"You don’t have a character yet. Use `/create` first.",
				ephemeral: true);
			return null;
		}

		if (player.ChannelId != context.Channel.Id.ToString())
		{
			await context.Interaction.RespondAsync(
				"You can only use commands in your private channel.",
				ephemeral: true);
			return null;
		}

		return player;
	}
}
