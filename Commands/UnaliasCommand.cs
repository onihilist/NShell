using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.Config;
using Spectre.Console;

namespace NShell.Commands;

public class UnaliasCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "unalias";
    public string Description => "Remove command aliases.";
    private static readonly ConfigManager _configManager = new ConfigManager();

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Usage: unalias name1 [name2 ...]");
            return;
        }

        foreach (var aliasName in args)
        {
            if (AliasCommand.Aliases.Remove(aliasName))
            {
                AnsiConsole.MarkupLine($"[[[green]+[/]]] - Removed alias: [yellow]{aliasName}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[[[yellow]*[/]]] - Alias not found: [yellow]{aliasName}[/]");
            }
        }
        
        _configManager.SaveAliases(AliasCommand.Aliases);
    }
}
