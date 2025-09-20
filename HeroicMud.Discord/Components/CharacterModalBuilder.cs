using Discord;
using Discord.Interactions;

namespace HeroicMud.Discord.Components;

/// <summary>
/// A Discord modal for creating a new character. Includes name, description and options for gender.
/// </summary>
public class CharacterModal : IModal
{
	public string Title => "Create Your Character";

	[ModalTextInput("name_input", placeholder: "Enter your character's name", maxLength: 30)]
	public string Name { get; set; } = string.Empty;

	[ModalTextInput("description_input", TextInputStyle.Paragraph, placeholder: "Describe your character", maxLength: 200)]
	public string Description { get; set; } = string.Empty;
}