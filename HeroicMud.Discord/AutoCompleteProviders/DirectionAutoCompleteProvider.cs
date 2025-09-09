using Discord;
using Discord.Interactions;

namespace HeroicMud.Discord.AutoCompleteProviders;

internal class DirectionAutoCompleteProvider : AutocompleteHandler
{
	public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		var directions = new List<string> { "north", "south", "east", "west", "up", "down", "northeast", "northwest", "southeast", "southwest" };
		var currentInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

		var results = directions
			.Where(d => d.StartsWith(currentInput, StringComparison.OrdinalIgnoreCase))
			.Select(d => new AutocompleteResult(d, d))
			.ToList();

		return Task.FromResult(AutocompletionResult.FromSuccess(results));
	}
}
