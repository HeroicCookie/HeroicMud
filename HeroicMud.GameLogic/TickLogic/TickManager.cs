namespace HeroicMud.GameLogic.TickLogic;

public class TickManager
{
	private readonly List<ITickable> _tickables = new();
	private readonly TimeSpan _tickInterval = TimeSpan.FromSeconds(3);
	private CancellationTokenSource? _cts;

	public void Register(ITickable tickable)
	{
		_tickables.Add(tickable);
	}

	public void Unregister(ITickable tickable)
	{
		_tickables.Remove(tickable);
	}

	public async Task StartAsync()
	{
		_cts = new CancellationTokenSource();
		while (!_cts.IsCancellationRequested)
		{
			Tick();
			await Task.Delay(_tickInterval, _cts.Token);
		}
	}

	public void Stop()
	{
		_cts?.Cancel();
	}

	private void Tick()
	{
		foreach (var tickable in _tickables.ToList())
		{
			tickable.OnTick();
		}
	}
}
