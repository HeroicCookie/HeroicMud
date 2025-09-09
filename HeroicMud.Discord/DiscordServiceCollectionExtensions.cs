using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace HeroicMud.Discord;

public static class DiscordServiceCollectionExtensions
{
	public static IServiceCollection AddDiscordServices(this IServiceCollection services)
	{
		services.AddSingleton(sp =>
			new DiscordSocketClient(new DiscordSocketConfig
			{
				GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages
			}));

		services.AddSingleton(sp =>
		{
			var client = sp.GetRequiredService<DiscordSocketClient>();
			return new InteractionService(client.Rest);
		});

		services.AddSingleton<LoggingService>();
		services.AddSingleton<DiscordBot>();

		return services;
	}
}
