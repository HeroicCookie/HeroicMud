using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

using static Discord.GatewayIntents;

namespace HeroicMud.Discord;

public class DiscordBot
{
	private readonly IServiceProvider _services;
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactionService;

	internal DiscordBot(IServiceProvider services, DiscordSocketClient client, InteractionService interactionService)
	{
		_services = services;
		_client = client;
		_interactionService = interactionService;

		_client.Log += LogAsync;
		_interactionService.Log += LogAsync;

		client.InteractionCreated += async i =>
		{
			var context = new SocketInteractionContext(_client, i);
			await interactionService.ExecuteCommandAsync(context, _services);
		};
	}

	public static async Task<DiscordBot> Create(IServiceProvider services)
	{
		DiscordSocketClient client = new(new() { GatewayIntents = Guilds | GuildMessages });
		DiscordBot discordBot = new(services, client, new(client.Rest));

		await discordBot._interactionService.AddModulesAsync(typeof(DiscordBot).Assembly, discordBot._services);
		client.Ready += async () => await discordBot._interactionService.RegisterCommandsGloballyAsync();

		return discordBot;
	}

	public async Task Start()
	{
		var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
			?? throw new InvalidOperationException("Environment variable DISCORD_TOKEN is unset.");

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();
	}

	public async Task Stop()
	{
		await _client.StopAsync();
	}

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
