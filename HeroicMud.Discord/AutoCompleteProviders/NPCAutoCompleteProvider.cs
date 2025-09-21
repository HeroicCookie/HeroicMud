using Discord;
using Discord.Interactions;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.Data.Rooms;

namespace HeroicMud.Discord.AutoCompleteProviders
{
	internal class NPCAutoCompleteProvider(MudGame game, RoomManager roomManager) : AutocompleteHandler
	{
		public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
		{
			Player player = game.GetPlayer(context.User.Id.ToString())!;
			Room? currentRoom = roomManager.GetRoom(player.CurrentRoomId);

			List<string> npcs = currentRoom?.Dialogues.Select(n => n.Name).ToList() ?? [];

			string currentInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

			List<AutocompleteResult>? results = npcs
				.Where(d => d.StartsWith(currentInput, StringComparison.OrdinalIgnoreCase))
				.Select(d => new AutocompleteResult(d, d))
				.ToList();

			return Task.FromResult(AutocompletionResult.FromSuccess(results));
		}
	}
}
