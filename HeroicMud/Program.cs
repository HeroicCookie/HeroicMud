using System.Data;
using HeroicMud.Discord;
using HeroicMud.Game;
using HeroicMud.Game.PlayerRepository;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
	private static async Task Main()
	{
		CancellationTokenSource cancellation = new();
		Console.CancelKeyPress += (_, _) => cancellation.Cancel();

		IPlayerRepository playerRepository;
		try
		{
			IDbConnection dbConnection = ConnectToPostgres();
			playerRepository = new PostgrePlayerRepository(dbConnection);
		}
		catch
		{
			playerRepository = new DummyPlayerRepository();
			Console.WriteLine("Could not connect to Postgres.");
			Console.WriteLine("USING DUMMY REPOSITORY!!!");
		}
		World world = new(playerRepository);
		await world.Start();

		ServiceProvider services = new ServiceCollection()
			.AddSingleton(world)
			.BuildServiceProvider();
		Bot bot = new(services);
		await bot.Start();

		await Task.Delay(Timeout.Infinite, cancellation.Token);

		await bot.Stop();
		await world.Stop();
	}

	private static IDbConnection ConnectToPostgres()
	{
		string connectionString = Environment.GetEnvironmentVariable("MUD_DB_CONNECTION")
			?? throw new InvalidOperationException("Environment variable MUD_DB_CONNECTION is unset.");

		var connection = new Npgsql.NpgsqlConnection(connectionString);
		connection.Open();

		return connection;
	}
}
