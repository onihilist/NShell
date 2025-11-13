using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class WhichCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "which";
    public string Description => "Locate a command and show its path.";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: which <command>");
            return;
        }

        foreach (var cmdName in args)
        {
            // Check if it's a custom command
            if (CommandParser.CustomCommands.ContainsKey(cmdName))
            {
                AnsiConsole.MarkupLine($"[green]{cmdName}[/]: [cyan]built-in shell command[/]");
                continue;
            }

            // Check if it's a system command
            if (CommandParser.SystemCommands.Contains(cmdName))
            {
                var paths = new[] { "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin" };
                bool found = false;

                foreach (var dir in paths)
                {
                    var fullPath = Path.Combine(dir, cmdName);
                    if (File.Exists(fullPath))
                    {
                        AnsiConsole.MarkupLine($"[cyan]{fullPath}[/]");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    AnsiConsole.MarkupLine($"[[[red]-[/]]] - {cmdName}: command not found");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - {cmdName}: command not found");
            }
        }
    }
}
