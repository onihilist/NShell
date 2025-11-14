using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class PrintEnvCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "printenv";
    public string Description => "Print environment variables.";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            // Display all environment variables
            var envVars = Environment.GetEnvironmentVariables();
            var sortedKeys = envVars.Keys.Cast<string>().OrderBy(k => k);
            
            foreach (var key in sortedKeys)
            {
                AnsiConsole.MarkupLine($"[cyan]{key}[/]=[yellow]{envVars[key]}[/]");
            }
        }
        else
        {
            // Display specific environment variables
            foreach (var varName in args)
            {
                var value = Environment.GetEnvironmentVariable(varName);
                if (value != null)
                {
                    AnsiConsole.MarkupLine($"[cyan]{varName}[/]=[yellow]{value}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[[[yellow]*[/]]] - Variable [cyan]{varName}[/] is not set.");
                }
            }
        }
    }
}
