using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class ExportCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "export";
    public string Description => "Set environment variables (e.g., export VAR=value).";

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
            return;
        }

        foreach (var arg in args)
        {
            var parts = arg.Split('=', 2);
            if (parts.Length != 2)
            {
                AnsiConsole.MarkupLine($"[[[yellow]*[/]]] - Invalid format: {arg}. Use: export VAR=value");
                continue;
            }

            string varName = parts[0].Trim();
            string varValue = parts[1].Trim();

            // Remove quotes if present
            if ((varValue.StartsWith("\"") && varValue.EndsWith("\"")) ||
                (varValue.StartsWith("'") && varValue.EndsWith("'")))
            {
                varValue = varValue.Substring(1, varValue.Length - 2);
            }

            Environment.SetEnvironmentVariable(varName, varValue);
            AnsiConsole.MarkupLine($"[[[green]+[/]]] - Set [cyan]{varName}[/]=[yellow]{varValue}[/]");
        }
    }
}
