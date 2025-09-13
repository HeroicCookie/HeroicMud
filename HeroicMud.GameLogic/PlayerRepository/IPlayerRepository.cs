using HeroicMud.GameLogic.Enums;

namespace HeroicMud.GameLogic.PlayerRepository;

public interface IPlayerRepository
{
	/// <summary>
	/// Gets a player by their Discord ID.
	/// </summary>
	/// <param name="discordId"></param>
	/// <returns>
	/// The player if found, null otherwise.
	/// </returns>
	Task<Player?> GetAsync(string discordId);

	/// <summary>
	/// Gets all players from the database.
	/// </summary>
	/// <returns>
	/// A list of all players.
	/// </returns>
	Task<List<Player>> GetAllAsync();

	/// <summary>
	/// Creates a player in the database.
	/// </summary>
	/// <param name="player"></param>
	/// <returns>
	/// True if the operation was successful, false otherwise.
	/// </returns>
	Task<SaveResult> CreateAsync(Player player);

	/// <summary>
	/// Updates an existing player in the database.
	/// </summary>
	/// <param name="player"></param>
	/// <returns>
	/// True if the operation was successful, false otherwise.
	/// </returns>
	Task<SaveResult> UpdateAsync(Player player);
}
