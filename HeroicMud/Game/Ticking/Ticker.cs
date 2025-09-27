namespace HeroicMud.Game.Ticking;

public class Ticker
{
	private readonly List<ITickable> _tickables = [];
	private readonly TimeSpan _tickInterval = TimeSpan.FromSeconds(3);

	private readonly CancellationTokenSource _tokenSource = new();
	private Task? _task;

	public void Register(ITickable tickable) => _tickables.Add(tickable);

	public void Unregister(ITickable tickable) => _tickables.Remove(tickable);

	public void Start()
	{
		_task = Task.Run(async () =>
			{
				while (!_tokenSource.Token.IsCancellationRequested)
				{
					Tick();
					await Task.Delay(_tickInterval, _tokenSource.Token);
				}
			}
		);
	}

	public async Task Stop()
	{
		_tokenSource.Cancel();
		if (_task is not null)
			await _task;
	}

	private void Tick()
	{
		foreach (var tickable in _tickables.ToList())
			tickable.OnTick();
	}
}
