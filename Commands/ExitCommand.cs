using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class ExitCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "exit";
    public string Description => "Exit the shell.";

    public void Execute(ShellContext context, string[] args)
    {
        // Save history before exiting
        Shell.Readline.ReadLine.History.Save();
        
        AnsiConsole.MarkupLine("[bold green]Goodbye![/]");
        Environment.Exit(0);
    }
}
