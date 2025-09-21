using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.GameLogic;

namespace HeroicMud.Discord.Handlers;

public static class DiscordHelpers
{
	public static async Task<ITextChannel?> CreatePrivateChannelAsync(SocketInteractionContext context, string name)
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

	public static async Task<Player?> GetVerifiedPlayerAsync(IInteractionContext context, MudGame game)
	{
		var player = game.GetPlayer(context.User.Id.ToString());

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
