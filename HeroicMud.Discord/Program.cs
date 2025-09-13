using HeroicMud.Discord;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.TickLogic;
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
		var tickManager = provider.GetRequiredService<TickManager>();
		var game = provider.GetRequiredService<MudGame>();

		_ = bot.StartAsync();
		_ = game.LoadPlayersAsync();
		_ = tickManager.StartAsync();

		await Task.Delay(-1);
	}
}
