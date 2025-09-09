using Dapper;
using HeroicMud.GameLogic.Enums;
using System.Data;

namespace HeroicMud.GameLogic.PlayerRepository;

public class PGPlayerRepository : IPlayerRepository
{
	private readonly IDbConnection _connection;

	public PGPlayerRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public async Task<Player?> GetAsync(string discordId)
	{
		return await _connection.QuerySingleOrDefaultAsync<Player>(
			"SELECT * FROM player WHERE discord_id = @discordId",
			new { discordId });
	}

	public async Task<SaveResult> CreateAsync(Player player)
	{
		try
		{
			var affected = await _connection.ExecuteAsync(
				@"INSERT INTO player (discord_id, name, gender)
              VALUES (@DiscordId, @Name, @Gender)
              ON CONFLICT (discord_id) DO NOTHING",
				player);

			if (affected == 0)
				return SaveResult.AlreadyExists;

			return SaveResult.Created;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error saving player: {ex.Message}");
			return SaveResult.Error;
		}
	}

	public async Task<SaveResult> UpdateAsync(Player player)
	{
		try
		{
			await _connection.ExecuteAsync(
				@"UPDATE player
				SET name = @Name
				WHERE discord_id = @DiscordId",
				player);
			return SaveResult.Updated;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error updating player: {ex.Message}");
			return SaveResult.Error;
		}

	}
}