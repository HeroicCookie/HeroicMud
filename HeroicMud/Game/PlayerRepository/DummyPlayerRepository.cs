using System.Data;
using Dapper;

namespace HeroicMud.Game.PlayerRepository;

public class DummyPlayerRepository : IPlayerRepository
{
	public override async Task<Player?> GetAsync(string discordId)
	{
		return await Task.FromResult<Player?>(null);
	}

	public override async Task<List<Player>> GetAllAsync()
	{
		return await Task.FromResult<List<Player>>([]);
	}

	public override async Task<SaveResult> CreateAsync(Player player)
	{
		return await Task.FromResult<SaveResult>(SaveResult.Created);
	}

	public override async Task<SaveResult> UpdateAsync(Player player)
	{
		return await Task.FromResult<SaveResult>(SaveResult.Updated);
	}
}