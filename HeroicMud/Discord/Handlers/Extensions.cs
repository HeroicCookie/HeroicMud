using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.Game;

namespace HeroicMud.Discord.Handlers;

public static class Extensions
{
    extension(SocketInteractionContext context)
    {
        public async Task<ITextChannel?> CreatePrivateChannelAsync(string name)
        {
            if (context.Channel is not SocketTextChannel parentChannel)
                return null;

            SocketGuild guild = parentChannel.Guild;
            ulong? categoryId = parentChannel.CategoryId;

            List<Overwrite> overwrites =
            [
                new(
                    guild.EveryoneRole.Id,
                    PermissionTarget.Role,
                    new OverwritePermissions(viewChannel: PermValue.Deny)),

                new(
                    context.User.Id,
                    PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow)),

                new(
                    guild.CurrentUser.Id,
                    PermissionTarget.User,
                    new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow))
            ];

            return await guild.CreateTextChannelAsync(name, props =>
            {
                props.CategoryId = categoryId;
                props.PermissionOverwrites = overwrites;
            });
        }
    }

    extension(IInteractionContext context)
    {
        public async Task<Player?> GetVerifiedPlayerAsync(World world)
        {
            Player? player = world.GetPlayer(context.User.Id.ToString());
            if (player is null)
            {
                await context.Interaction.RespondAsync(
                    "You don’t have a character yet. Use `/create` first.",
                    ephemeral: true);
            }
            else if (player.ChannelId != context.Channel.Id.ToString())
            {
                await context.Interaction.RespondAsync(
                    "You can only use commands in your private channel.",
                    ephemeral: true);
            }

            return player;
        }
    }
}
