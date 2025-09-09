using Discord;
using Discord.Interactions;
using HeroicMud.Discord.AutoCompleteProviders;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Enums;

namespace HeroicMud.Discord;

public class InteractionHandler : InteractionModuleBase<SocketInteractionContext>
{
	private readonly MudGame _game;

	public InteractionHandler(MudGame game)
	{
		_game = game;
	}

	[SlashCommand("look", "Look around in the current room.")]
	public async Task LookAsync()
	{
		string discordId = Context.User.Id.ToString();

		string output = _game.HandlePlayerCommand(discordId, GameCommand.Look).Result;
		await RespondAsync(output);
	}

	[SlashCommand("go", "Move in a direction.")]
	public async Task GoAsync(
		[Autocomplete(typeof(DirectionAutoCompleteProvider))]
		[Summary("direction", "The direction to move.")] string direction)
	{
		string discordId = Context.User.Id.ToString();

		string output = _game.HandlePlayerCommand(discordId, GameCommand.Go, direction).Result;
		await RespondAsync(output);
	}

	[SlashCommand("say", "Say something to others in the room.")]
	public async Task SayAsync([Summary("message", "The message to say.")] string message)
	{
		string discordId = Context.User.Id.ToString();

		string output = _game.HandlePlayerCommand(discordId, GameCommand.Say, message).Result;
		await RespondAsync(output);
	}

	[SlashCommand("create", "Create a new character.")]
	public async Task CreateAsync([Summary("name", "Name thyself.")] string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			await RespondAsync("Name cannot be empty.");
			return;
		}

		SaveResult result = await _game.CreatePlayerAsync(Context.User.Id.ToString(), name, 'm');

		switch (result)
		{
			case SaveResult.Created:
				await RespondAsync($"Character '{name}' created!", ephemeral: true);
				break;

			case SaveResult.AlreadyExists:
				await RespondAsync("You already have a character.", ephemeral: true);
				break;

			case SaveResult.Error:
			default:
				await RespondAsync("Failed to create character. Try again later.", ephemeral: true);
				break;
		}

	}
}
