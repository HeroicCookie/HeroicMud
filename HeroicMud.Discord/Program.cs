using HeroicMud.Discord;
using HeroicMud.GameLogic;
using Microsoft.Extensions.DependencyInjection;

class Program
{
	static async Task Main(string[] args)
	{
		var services = new ServiceCollection();

		services.AddGameLogic();
		services.AddDiscordServices();

		var provider = services.BuildServiceProvider();

		provider.GetRequiredService<LoggingService>();
		var bot = provider.GetRequiredService<DiscordBot>();

		await bot.StartAsync();

		await Task.Delay(-1);
	}
}
