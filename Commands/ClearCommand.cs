using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class ClearCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "clear";
    public string Description => "Clear the terminal screen.";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.Clear();
    }
}
