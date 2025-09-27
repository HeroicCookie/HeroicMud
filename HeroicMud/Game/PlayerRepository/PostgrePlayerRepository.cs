using System.Data;
using Dapper;

namespace HeroicMud.Game.PlayerRepository;

public class PostgrePlayerRepository(IDbConnection dbConnection) : IPlayerRepository
{
	public override async Task<Player?> GetAsync(string discordId)
	{
		return await dbConnection.QuerySingleOrDefaultAsync<Player>(
			"SELECT * FROM player WHERE discord_id = @discordId",
			new { discordId }
		);
	}

	public override async Task<List<Player>> GetAllAsync()
	{
		var players = await dbConnection.QueryAsync<Player>("""
			SELECT
				discord_id AS DiscordId,
				channel_id AS ChannelId,
				name,
				gender,
				current_room_id AS CurrentRoomId
			FROM player
			"""
		);

		return [.. players];
	}

	public override async Task<SaveResult> CreateAsync(Player player)
	{
		try
		{
			var affected = await dbConnection.ExecuteAsync("""
				INSERT INTO player (discord_id, channel_id, name, description, gender, current_room_id)
				VALUES (@DiscordId, @ChannelId, @Name, @Description, @Gender, @CurrentRoomId)
				ON CONFLICT (discord_id) DO NOTHING
				""",
				player
			);

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
			await dbConnection.ExecuteAsync("""
				UPDATE player
				SET name = @Name, current_room_id = @CurrentRoomId
				WHERE discord_id = @DiscordId
				""",
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