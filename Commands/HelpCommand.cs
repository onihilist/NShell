using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class HelpCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "help";
    public string Description => "Display help information about available commands.";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.MarkupLine("[bold cyan]NShell - Available Commands[/]\n");
        
        var table = new Table();
        table.AddColumn("[bold]Command[/]");
        table.AddColumn("[bold]Description[/]");
        
        // Add custom commands with descriptions
        foreach (var cmd in CommandParser.CustomCommands.Values.OrderBy(c => c.Name))
        {
            string description = "No description available";
            if (cmd is IMetadataCommand metaCmd)
            {
                description = metaCmd.Description;
            }
            
            table.AddRow($"[yellow]{cmd.Name}[/]", description);
        }
        
        AnsiConsole.Write(table);
        
        AnsiConsole.MarkupLine($"\n[grey]Total custom commands: {CommandParser.CustomCommands.Count}[/]");
        AnsiConsole.MarkupLine($"[grey]Total system commands: {CommandParser.SystemCommands.Count}[/]");
        AnsiConsole.MarkupLine("\n[grey]Type a command name to execute it, or use Tab for auto-completion.[/]");
    }
}
