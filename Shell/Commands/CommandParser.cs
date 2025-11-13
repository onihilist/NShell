using Spectre.Console;
using System.Diagnostics;

namespace NShell.Shell.Commands;

/// <summary>
/// <c>CommandParser</c> class handles loading and execution of shell commands.
/// It supports custom commands, system commands, and privilege management (e.g., sudo).
/// </summary>
public class CommandParser
{
    public static readonly Dictionary<string, ICustomCommand> CustomCommands = new();
    public static readonly HashSet<string> SystemCommands = new();
    private static readonly HashSet<string> InteractiveCommands = new()
    {
        "vim", "nano", "less", "more", "top", "htop", "man", "ssh", "apt"
    };

    /// <summary>
    /// Constructor that loads the commands when the parser is instantiated.
    /// </summary>
    public CommandParser()
    {
        LoadCommands();
    }

    /// <summary>
    /// Loads all the custom commands and system commands from predefined directories.
    /// </summary>
    private void LoadCommands()
    {
        foreach (var command in CommandRegistry.GetAll())
        {
            CustomCommands[command.Name] = command;
            AnsiConsole.MarkupLine($"\t[[[green]+[/]]] - Loaded custom command: [yellow]{command.Name}[/]");
        }

        LoadSystemCommands();

        var total = CustomCommands.Count + SystemCommands.Count;

        AnsiConsole.MarkupLine(total > 0
            ? $"[bold grey]→ Total commands loaded:[/] [bold green]{total}[/]"
            : $"[bold grey]→ Total commands loaded:[/] [yellow]{total}[/]");
    }

    /// <summary>
    /// Loads system commands from typical binary directories.
    /// </summary>
    private void LoadSystemCommands()
    {
        var paths = new[] { "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin" };

        foreach (var dir in paths)
        {
            if (!Directory.Exists(dir)) continue;

            foreach (var file in Directory.GetFiles(dir))
            {
                var cmd = Path.GetFileName(file);
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    SystemCommands.Add(cmd);
                }
            }
        }
    }

    /// <summary>
    /// Attempts to execute a command from the provided command line input.
    /// Handles variable expansion, root privileges, piping, redirection, chaining, and command execution.
    /// </summary>
    /// <param name="commandLine">The command line string entered by the user.</param>
    /// <param name="context">The shell context that contains environment variables and the current directory.</param>
    /// <returns>Returns true if the command was successfully executed, false otherwise.</returns>
    public bool TryExecute(string commandLine, ShellContext context)
    {
        var expanded = context.ExpandVariables(commandLine);
        
        // Handle command chaining (&&, ||, ;)
        if (expanded.Contains("&&") || expanded.Contains("||") || expanded.Contains(';'))
        {
            return ExecuteChainedCommands(expanded, context);
        }
        
        // Handle piping (|)
        if (expanded.Contains('|'))
        {
            return ExecutePipeline(expanded, context);
        }
        
        // Handle output redirection (>, >>)
        string? redirectFile = null;
        bool appendMode = false;
        
        if (expanded.Contains(">>"))
        {
            var redirectParts = expanded.Split(">>", 2);
            expanded = redirectParts[0].Trim();
            redirectFile = redirectParts[1].Trim();
            appendMode = true;
        }
        else if (expanded.Contains('>'))
        {
            var redirectParts = expanded.Split('>', 2);
            expanded = redirectParts[0].Trim();
            redirectFile = redirectParts[1].Trim();
            appendMode = false;
        }
        
        // Redirect output if needed
        TextWriter? originalOut = null;
        StreamWriter? fileWriter = null;
        
        if (redirectFile != null)
        {
            try
            {
                originalOut = Console.Out;
                fileWriter = new StreamWriter(redirectFile, appendMode);
                Console.SetOut(fileWriter);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[[[red]-[/]]] - Error opening file for redirection: {ex.Message}");
                return true;
            }
        }
        
        try
        {
            return ExecuteSingleCommand(expanded, context);
        }
        finally
        {
            // Restore original output
            if (originalOut != null)
            {
                Console.SetOut(originalOut);
                fileWriter?.Close();
            }
        }
    }

    /// <summary>
    /// Executes chained commands with &&, ||, or ;
    /// </summary>
    private bool ExecuteChainedCommands(string commandLine, ShellContext context)
    {
        // Split by ; first (unconditional execution)
        var semicolonParts = SplitByOperator(commandLine, ';');
        
        foreach (var part in semicolonParts)
        {
            var trimmedPart = part.Trim();
            if (string.IsNullOrWhiteSpace(trimmedPart)) continue;
            
            // Check for && (execute if previous succeeded)
            if (trimmedPart.Contains("&&"))
            {
                var andParts = SplitByOperator(trimmedPart, "&&");
                bool previousSuccess = true;
                
                foreach (var andPart in andParts)
                {
                    if (!previousSuccess) break;
                    
                    var cmd = andPart.Trim();
                    if (!string.IsNullOrWhiteSpace(cmd))
                    {
                        previousSuccess = TryExecute(cmd, context);
                    }
                }
            }
            // Check for || (execute if previous failed)
            else if (trimmedPart.Contains("||"))
            {
                var orParts = SplitByOperator(trimmedPart, "||");
                bool previousSuccess = false;
                
                foreach (var orPart in orParts)
                {
                    if (previousSuccess) break;
                    
                    var cmd = orPart.Trim();
                    if (!string.IsNullOrWhiteSpace(cmd))
                    {
                        previousSuccess = TryExecute(cmd, context);
                    }
                }
            }
            else
            {
                TryExecute(trimmedPart, context);
            }
        }
        
        return true;
    }

    /// <summary>
    /// Splits a string by an operator, respecting quotes.
    /// </summary>
    private List<string> SplitByOperator(string input, string op)
    {
        var parts = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (c == '"' || c == '\'')
            {
                inQuotes = !inQuotes;
                current.Append(c);
            }
            else if (!inQuotes && i + op.Length <= input.Length && input.Substring(i, op.Length) == op)
            {
                parts.Add(current.ToString());
                current.Clear();
                i += op.Length - 1;
            }
            else
            {
                current.Append(c);
            }
        }
        
        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }
        
        return parts;
    }

    /// <summary>
    /// Splits a string by a single character operator.
    /// </summary>
    private List<string> SplitByOperator(string input, char op)
    {
        return SplitByOperator(input, op.ToString());
    }

    /// <summary>
    /// Executes a pipeline of commands separated by pipes.
    /// </summary>
    private bool ExecutePipeline(string pipeline, ShellContext context)
    {
        var commands = pipeline.Split('|').Select(c => c.Trim()).ToArray();
        
        if (commands.Length < 2)
        {
            return ExecuteSingleCommand(pipeline, context);
        }

        AnsiConsole.MarkupLine("[[[yellow]*[/]]] - Piping is partially supported. Complex pipelines may not work as expected.");
        
        // For now, just execute each command independently
        // Full pipe implementation would require capturing stdout and passing to stdin
        foreach (var cmd in commands)
        {
            ExecuteSingleCommand(cmd, context);
        }
        
        return true;
    }

    /// <summary>
    /// Executes a single command without piping or redirection.
    /// </summary>
    private bool ExecuteSingleCommand(string commandLine, ShellContext context)
    {
        // Check for alias expansion
        var parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 0 && NShell.Commands.AliasCommand.Aliases.TryGetValue(parts[0], out var aliasValue))
        {
            // Replace alias with its value and append remaining arguments
            commandLine = aliasValue;
            if (parts.Length > 1)
            {
                commandLine += " " + string.Join(' ', parts.Skip(1));
            }
            parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        
        if (parts.Length == 0) return false;

        var usedSudo = parts[0] == "sudo";
        if (usedSudo) parts = parts.Skip(1).ToArray();

        if (parts.Length == 0) return false;

        var cmdName = parts[0];
        var args = parts.Skip(1).ToArray();

        if (cmdName.StartsWith("./"))
        {
            return ExecuteLocalFile(commandLine);
        }

        if (CustomCommands.TryGetValue(cmdName, out var customCmd))
        {
            if (customCmd is IMetadataCommand meta && meta.RequiresRoot && !(usedSudo || IsRootUser()))
            {
                AnsiConsole.MarkupLine("[red][[-]] - This command requires root privileges. Prefix with [bold yellow]sudo[/] or run as root.[/]");
                return true;
            }

            customCmd.Execute(context, args);
            return true;
        }

        if (SystemCommands.Contains(cmdName))
        {
            var fullPath = ResolveSystemCommandPath(cmdName);
            if (fullPath != null)
            {
                RunSystemCommand(fullPath, args, usedSudo);

                if ((cmdName == "apt" || cmdName == "apt-get") && args.Length > 0 && args[0] == "install")
                {
                    CommandLoader.RefreshCommands();
                }

                return true;
            }
        }

        AnsiConsole.MarkupLine($"[[[red]-[/]]] - Unknown command: [bold yellow]{cmdName}[/]");
        return true;
    }

    /// <summary>
    /// Executes a local shell file.
    /// </summary>
    private static bool ExecuteLocalFile(string commandLine)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{commandLine}\"",
                    UseShellExecute = false
                }
            };

            process.Start();
            process.WaitForExit();

            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[[[red]-[/]]] - Error executing file: {ex.Message}");
            return true;
        }
    }

    /// <summary>
    /// Resolves the full path of a system command by searching in common system directories.
    /// </summary>
    private static string? ResolveSystemCommandPath(string cmdName)
    {
        var paths = new[] { "/usr/bin", "/usr/local/bin", "/usr/games", "/bin", "/sbin", "/usr/sbin" };

        return paths.Select(path => Path.Combine(path, cmdName))
                    .FirstOrDefault(fullPath => File.Exists(fullPath) && IsExecutable(fullPath));
    }

    /// <summary>
    /// Checks if a file is executable.
    /// </summary>
    private static bool IsExecutable(string path)
    {
        return new FileInfo(path).Exists;
    }

    /// <summary>
    /// Runs a system command, optionally using `sudo`, and handles interactive vs non-interactive command behavior.
    /// </summary>
    private static bool RunSystemCommand(string path, string[] args, bool useSudo)
    {
        var isInteractive = InteractiveCommands.Contains(Path.GetFileName(path));
        var startInfo = new ProcessStartInfo
        {
            FileName = useSudo ? "/usr/bin/sudo" : path,
            Arguments = useSudo ? $"{path} {string.Join(' ', args)}" : string.Join(' ', args),
            UseShellExecute = isInteractive,
            RedirectStandardOutput = !isInteractive,
            RedirectStandardError = !isInteractive,
            CreateNoWindow = false
        };

        using var process = new Process { StartInfo = startInfo };

        if (!isInteractive)
        {
            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data)) Console.WriteLine(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Data);
                    Console.ResetColor();
                }
            };
        }

        process.Start();

        if (!isInteractive)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        process.WaitForExit();
        return process.ExitCode == 0;
    }

    /// <summary>
    /// Checks if the current user is root.
    /// </summary>
    private static bool IsRootUser()
    {
        return Environment.UserName == "root" || Environment.GetEnvironmentVariable("USER") == "root";
    }
}
