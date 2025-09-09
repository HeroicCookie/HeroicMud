using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HeroicMud.GameLogic;
using HeroicMud.GameLogic.PlayerRepository;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace HeroicMud.Discord;

public class DiscordBot
{
	private readonly MudGame _game;
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactions;
	private readonly IServiceProvider _services;

	public DiscordBot(MudGame game, DiscordSocketClient client, InteractionService interactions, IServiceProvider services)
	{
		_game = game;
		_client = client;
		_interactions = interactions;
		_services = services;
	}

	public async Task StartAsync()
	{
		await _interactions.AddModulesAsync(typeof(DiscordBot).Assembly, _services);

		_client.InteractionCreated += async arg =>
		{
			var ctx = new SocketInteractionContext(_client, arg);
			await _interactions.ExecuteCommandAsync(ctx, _services);
		};

		_client.Ready += async () =>
		{
			await _interactions.RegisterCommandsGloballyAsync();
		};

		var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") 
			?? throw new InvalidOperationException("DISCORD_TOKEN env var not set");

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.StartAsync();

		await Task.Delay(-1);
	}
}
