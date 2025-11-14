using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class AboutCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "about";
    public string Description => "Display information about NShell.";

    public void Execute(ShellContext context, string[] args)
    {
        AnsiConsole.Clear();
        
        var panel = new Panel(new Markup(
            $"[bold cyan]NShell[/] - A Custom C# Interactive Shell\n\n" +
            $"[grey]Version:[/] [yellow]{Program.VERSION}[/]\n" +
            $"[grey]GitHub:[/] [blue]{Program.GITHUB}[/]\n\n" +
            $"[grey]Runtime:[/] [green].NET {Environment.Version}[/]\n" +
            $"[grey]Platform:[/] [green]{Environment.OSVersion}[/]\n\n" +
            $"[dim]Type [yellow]help[/] to see available commands.[/]"
        ))
        {
            Header = new PanelHeader("[bold green] About NShell [/]"),
            Border = BoxBorder.Rounded
        };
        
        AnsiConsole.Write(panel);
        Console.WriteLine();
    }
}
