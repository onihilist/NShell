using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class UnsetCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "unset";
    public string Description => "Remove environment variables.";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: unset VAR1 [VAR2 ...]");
            return;
        }

        foreach (var varName in args)
        {
            Environment.SetEnvironmentVariable(varName, null);
            AnsiConsole.MarkupLine($"[[[green]+[/]]] - Unset [cyan]{varName}[/]");
        }
    }
}
