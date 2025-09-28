using System.Collections.Concurrent;
using System.Threading.Channels;
using HeroicMud.Game.Content.Rooms;
using HeroicMud.Game.PlayerRepository;

namespace HeroicMud.Game;

public class World(IPlayerRepository playerRepository)
{
    public readonly RoomManager RoomManager = new();

    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly ConcurrentBag<Player> _players = [];

    private readonly Channel<PlayerCommand> _commandChannel = Channel.CreateUnbounded<PlayerCommand>();
    private Task? _processTask;

    public async Task Start()
    {
        (await _playerRepository.GetAllAsync()).ForEach(p => _players.Add(p));
        _processTask = Task.Run(async () =>
        {
            while (!_commandChannel.Reader.Completion.IsCompleted)
                await (await _commandChannel.Reader.ReadAsync()).Do(this);
        });
    }

    public async Task Stop()
    {
        _commandChannel.Writer.Complete();
        if (_processTask is not null)
            await _processTask;
    }

    public async Task<string> SendPlayerCommand(PlayerCommand command)
    {
        await _commandChannel.Writer.WriteAsync(command);
        return await command.Description;
    }

    public async Task<SaveResult> CreatePlayerAsync(string discordId, string channelId, string name, string description)
    {
        Player player = new()
        {
            DiscordId = discordId,
            ChannelId = channelId,
            Name = name,
            Description = description,
            CurrentRoomId = "windstop_inn_common_room"
        };
        _players.Add(player);

        return await _playerRepository.CreateAsync(player);
    }

    public async Task<SaveResult> SavePlayerAsync(Player player) => await _playerRepository.UpdateAsync(player);

    public Player? GetPlayer(string discordId) => _players.FirstOrDefault(p => p.DiscordId == discordId);

    public bool PlayerExists(string discordId) => GetPlayer(discordId) != null;

    public IEnumerable<Player> GetPlayersInRoom(string roomId) => _players.Where(p => p.CurrentRoomId == roomId);
}
