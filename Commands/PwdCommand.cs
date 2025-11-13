using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class PwdCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "pwd";
    public string Description => "Print the current working directory.";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.MarkupLine($"[cyan]{Directory.GetCurrentDirectory()}[/]");
    }
}
