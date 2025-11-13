using NShell.Shell;
using NShell.Shell.Commands;
using Spectre.Console;

namespace NShell.Commands;

public class EchoCommand : ICustomCommand, IMetadataCommand
{
    public string Name => "echo";
    public string Description => "Display a line of text.";

    public void Execute(ShellContext context, string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine();
            return;
        }

        // Check for -n flag (no trailing newline)
        bool noNewline = false;
        int startIndex = 0;

        if (args[0] == "-n")
        {
            noNewline = true;
            startIndex = 1;
        }

        // Join remaining args with spaces and expand variables
        string output = string.Join(' ', args.Skip(startIndex));
        output = context.ExpandVariables(output);

        if (noNewline)
        {
            Console.Write(output);
        }
        else
        {
            Console.WriteLine(output);
        }
    }
}
