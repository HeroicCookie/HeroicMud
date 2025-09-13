using Discord;
using Discord.Interactions;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Room;

namespace HeroicMud.Discord.AutoCompleteProviders;

internal class DirectionAutoCompleteProvider : AutocompleteHandler
{
	private readonly MudGame _game;
	private readonly RoomManager _roomManager;

	public DirectionAutoCompleteProvider(MudGame game, RoomManager roomManager)
	{
		_game = game;
		_roomManager = roomManager;
	}

	public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		Player player = _game.GetPlayer(context.User.Id.ToString());
		IRoom? currentRoom = _roomManager.GetRoom(player.CurrentRoomId);

		var exits = currentRoom?.Exits.Keys.ToList() ?? [];

		var currentInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

		var results = exits
			.Where(d => d.StartsWith(currentInput, StringComparison.OrdinalIgnoreCase))
			.Select(d => new AutocompleteResult(d, d))
			.ToList();

		return Task.FromResult(AutocompletionResult.FromSuccess(results));
	}
}
