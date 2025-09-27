using Discord;
using Discord.Interactions;
using HeroicMud.Game;
using HeroicMud.Game.Content.Rooms;

namespace HeroicMud.Discord.AutoCompleteProviders
{
	internal class NPCAutoCompleteProvider(Game.World world) : AutocompleteHandler
	{
		public override Task<AutocompletionResult> GenerateSuggestionsAsync(
			IInteractionContext context,
			IAutocompleteInteraction autocompleteInteraction,
			IParameterInfo parameter,
			IServiceProvider services)
		{
			Player player = world.GetPlayer(context.User.Id.ToString())!;
			Room? currentRoom = world.RoomManager.GetRoom(player.CurrentRoomId);

			List<string> npcs = currentRoom?.GetNPCs(player).Select(n => n.Name).ToList() ?? [];
			string currentInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

			List<AutocompleteResult>? results = [.. npcs
				.Where(d => d.StartsWith(currentInput, StringComparison.OrdinalIgnoreCase))
				.Select(d => new AutocompleteResult(d, d))];

			return Task.FromResult(AutocompletionResult.FromSuccess(results));
		}
	}
}
