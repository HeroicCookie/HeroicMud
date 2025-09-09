using HeroicMud.GameLogic.PlayerRepository;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace HeroicMud.GameLogic;

public static class GameLogicServiceCollectionExtensions
{
	public static IServiceCollection AddGameLogic(this IServiceCollection services)
	{
		services.AddSingleton<MudGame>();
		services.AddSingleton<IPlayerRepository, PGPlayerRepository>();
		services.AddSingleton<IDbConnection>(sp =>
		{
			var conn = new Npgsql.NpgsqlConnection(
				Environment.GetEnvironmentVariable("MUD_DB_CONNECTION")
				?? throw new InvalidOperationException("MUD_DB_CONNECTION not set"));
			conn.Open();
			return conn;
		});

		return services;
	}
}
