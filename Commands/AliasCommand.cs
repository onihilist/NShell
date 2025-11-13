using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class AliasCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "alias";
    public string Description => "Create command aliases (e.g., alias ll='ls -la').";
    
    // Static dictionary to store aliases
    public static Dictionary<string, string> Aliases { get; } = new Dictionary<string, string>();

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            // List all aliases
            if (Aliases.Count == 0)
            {
                AnsiConsole.MarkupLine("[[[yellow]*[/]]] - No aliases defined.");
                return;
            }

            AnsiConsole.MarkupLine("[bold cyan]Current Aliases:[/]\n");
            foreach (var alias in Aliases.OrderBy(a => a.Key))
            {
                AnsiConsole.MarkupLine($"[yellow]{alias.Key}[/]=[green]'{alias.Value}'[/]");
            }
            return;
        }

        // Join all args to handle aliases with spaces
        var fullArg = string.Join(' ', args);
        var parts = fullArg.Split('=', 2);

        if (parts.Length != 2)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: alias name='command'");
            return;
        }

        string aliasName = parts[0].Trim();
        string aliasValue = parts[1].Trim();

        // Remove quotes if present
        if ((aliasValue.StartsWith("\"") && aliasValue.EndsWith("\"")) ||
            (aliasValue.StartsWith("'") && aliasValue.EndsWith("'")))
        {
            aliasValue = aliasValue.Substring(1, aliasValue.Length - 2);
        }

        Aliases[aliasName] = aliasValue;
        AnsiConsole.MarkupLine($"[[[green]+[/]]] - Alias created: [yellow]{aliasName}[/]=[green]'{aliasValue}'[/]");
    }
}
