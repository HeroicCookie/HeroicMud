using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace HeroicMud.Discord;

internal class LoggingService
{
	public LoggingService(DiscordSocketClient client, InteractionService command)
	{
		client.Log += LogAsync;
		command.Log += LogAsync;
	}

	private Task LogAsync(LogMessage message)
    {
        Console.WriteLine($"[{message.Source}/{message.Severity}] {message}");
        if (message.Exception is CommandException cmdEx)
        {
            Console.WriteLine($"Command {cmdEx.Command.Name} failed in {cmdEx.Context.Channel}");
            Console.WriteLine(cmdEx);
        }
        return Task.CompletedTask;
    }
}
