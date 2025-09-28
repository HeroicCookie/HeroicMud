using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using static Discord.GatewayIntents;

namespace HeroicMud.Discord;

public class Bot
{
	private readonly IServiceProvider _services;
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactionService;

	public Bot(IServiceProvider services)
	{
		_services = services;
		_client = new(new() { GatewayIntents = Guilds | GuildMessages });
		_interactionService = new(_client.Rest);

		_client.Log += LogAsync;
		_interactionService.Log += LogAsync;
		_client.Ready += async () => await _interactionService.RegisterCommandsGloballyAsync();
		_client.InteractionCreated += async i =>
		{
			var context = new SocketInteractionContext(_client, i);
			await _interactionService.ExecuteCommandAsync(context, services);
		};
	}

	public async Task Start()
	{
		await _interactionService.AddModulesAsync(typeof(Bot).Assembly, _services);

		await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN")
			?? throw new InvalidOperationException("Environment variable DISCORD_TOKEN is unset."));

		await _client.StartAsync();
	}

	public async Task Stop() => await _client.StopAsync();

	private Task LogAsync(LogMessage message)
	{
		Console.WriteLine($"[{message.Source}/{message.Severity}] {message}");
		if (message.Exception is CommandException e)
		{
			Console.WriteLine($"Command {e.Command.Name} failed in {e.Context.Channel}");
			Console.WriteLine(e);
		}

		return Task.CompletedTask;
	}
}
