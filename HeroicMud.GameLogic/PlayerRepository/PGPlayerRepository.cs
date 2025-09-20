using System.Data;
using Dapper;
using HeroicMud.GameLogic.Enums;

namespace HeroicMud.GameLogic.PlayerRepository;

public class PGPlayerRepository : IPlayerRepository
{
	private readonly IDbConnection _connection;

	public PGPlayerRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public override async Task<Player?> GetAsync(string discordId)
	{
		return await _connection.QuerySingleOrDefaultAsync<Player>(
			"SELECT * FROM player WHERE discord_id = @discordId",
			new { discordId });
	}

	public override async Task<List<Player>> GetAllAsync()
	{
		var players = await _connection.QueryAsync<Player>(@"
		SELECT
			discord_id AS DiscordId,
			channel_id AS ChannelId,
			name,
			gender,
			current_room_id AS CurrentRoomId
		FROM player"
		);
		return players.ToList();
	}

	public override async Task<SaveResult> CreateAsync(Player player)
	{
		try
		{
			var affected = await _connection.ExecuteAsync(
				@"INSERT INTO player (discord_id, channel_id, name, description, gender, current_room_id)
              VALUES (@DiscordId, @ChannelId, @Name, @Description, @Gender, @CurrentRoomId)
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

	public override async Task<SaveResult> UpdateAsync(Player player)
	{
		try
		{
			await _connection.ExecuteAsync(
				@"UPDATE player
				SET name = @Name, current_room_id = @CurrentRoomId
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