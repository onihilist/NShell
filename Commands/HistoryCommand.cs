using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.Readline;
using Spectre.Console;

namespace NShell.Commands;

public class HistoryCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "history";
    public string Description => "Display command history.";

    public void Execute(ShellContext context, string[] args)
    {
        int displayCount = 20; // Default to last 20 commands
        int historyCount = ReadLine.History.Count;

        if (args.Length > 0)
        {
            if (args[0] == "-c" || args[0] == "--clear")
            {
                // Clear history - not implemented as it would require HistoryManager changes
                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - History clearing not yet implemented.");
                return;
            }
            else if (int.TryParse(args[0], out int count))
            {
                displayCount = count;
            }
            else
            {
                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: history [number] or history -c");
                return;
            }
        }

        // Display the last N commands
        int startIndex = Math.Max(0, historyCount - displayCount);
        
        if (historyCount == 0)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - No commands in history.");
            return;
        }

        AnsiConsole.MarkupLine($"[bold cyan]Command History (last {Math.Min(displayCount, historyCount)} commands):[/]\n");

        for (int i = startIndex; i < historyCount; i++)
        {
            var command = ReadLine.History.GetAt(i);
            if (command != null)
            {
                AnsiConsole.MarkupLine($"  [grey]{i + 1,4}[/]  {command}");
            }
        }
    }
}
