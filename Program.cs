
using Spectre.Console;
using NShell.Shell;
using NShell.Shell.Commands;
using NShell.Shell.Plugins;
using NShell.Shell.Readline;
using static NShell.Animation.GlitchOutput;

public class Program
{
    public static readonly string VERSION = "0.3.0-pre";
    public static readonly string GITHUB = "https://github.com/onihilist/NShell";

    public static async Task Main(string[] args)
    {
        bool noBanner = false;
        
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "--version":
                case "-v":
                    Console.WriteLine($"NShell {VERSION}");
                    return;
                case "--help":
                case "-h":
                    Console.WriteLine("Usage: nshell [--version | --help | --no-banner]");
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("  --version, -v     Show version information");
                    Console.WriteLine("  --help, -h        Show this help message");
                    Console.WriteLine("  --no-banner       Start without the welcome banner");
                    return;
                case "--no-banner":
                    noBanner = true;
                    break;
            }
        }

        AnsiConsole.Clear();
        
        if (!noBanner)
        {
            AnsiConsole.Markup($"Welcome {Environment.UserName} to NShell !\n\n");
            AnsiConsole.Markup($"\tversion : {VERSION}\n");
            AnsiConsole.Markup($"\tgithub  : {GITHUB}\n");
            AnsiConsole.Markup("\n");
        }

        AnsiConsole.Markup("[bold cyan][[*]] - Booting NShell...[/]\n");
        ShellContext context = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading command(s)...[/]\n");
        CommandParser parser = new();
        PluginLoader plugins = new();
        AnsiConsole.Markup("[bold cyan][[*]] - Loading plugin(s)...[/]\n");
        plugins.LoadPlugins();
        parser.LoadCommands();
        
        AppDomain.CurrentDomain.ProcessExit += (_, _) => {
            ReadLine.History.Save();
        };

        if (!noBanner)
        {
            await GlitchedPrint("[+] - System Online", TimeSpan.FromMilliseconds(20));
        }
        else
        {
            AnsiConsole.MarkupLine("[bold green][[+]] - System Online[/]");
        }
        
        string inputBuffer;

        while (true)
        {
            Environment.SetEnvironmentVariable("LS_COLORS", context.GetLsColors());
            context.SetTheme(context.CurrentTheme);
            AnsiConsole.Markup(context.GetPrompt());
            ReadLine.History.ResetIndex();
            ReadLine.Handler.UpdateInitCursorPos(Console.CursorLeft);

            inputBuffer = ReadLine.GetText();

            if (string.IsNullOrWhiteSpace(inputBuffer)) continue;
            ReadLine.History.Add(inputBuffer);

            try
            {
                parser.TryExecute(inputBuffer, context);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Shell crash: [yellow]{ex.Message}[/]");
            }
        }
    }
}
