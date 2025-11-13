using System.Text;
using Spectre.Console;

namespace NShell.Shell.History;

/// <summary>
/// Provides interactive reverse history search functionality.
/// </summary>
public class HistorySearch
{
    private readonly HistoryManager _history;

    public HistorySearch(HistoryManager history)
    {
        _history = history;
    }

    /// <summary>
    /// Performs an interactive reverse history search.
    /// Returns the selected command or null if cancelled.
    /// </summary>
    public string? Search(int initialCursorLeft)
    {
        var searchBuffer = new StringBuilder();
        var matches = new List<string>();
        int matchIndex = 0;

        // Display initial search prompt
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("(reverse-i-search)`': ");
        Console.ResetColor();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                // Accept current match
                Console.WriteLine();
                return matches.Count > 0 ? matches[matchIndex] : null;
            }
            else if (key.Key == ConsoleKey.Escape || (key.Key == ConsoleKey.G && key.Modifiers.HasFlag(ConsoleModifiers.Control)))
            {
                // Cancel search
                Console.WriteLine();
                return null;
            }
            else if (key.Key == ConsoleKey.R && key.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                // Next match
                if (matches.Count > 0)
                {
                    matchIndex = (matchIndex + 1) % matches.Count;
                }
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (searchBuffer.Length > 0)
                {
                    searchBuffer.Remove(searchBuffer.Length - 1, 1);
                    UpdateSearch(searchBuffer.ToString(), out matches, out matchIndex);
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                searchBuffer.Append(key.KeyChar);
                UpdateSearch(searchBuffer.ToString(), out matches, out matchIndex);
            }
            else
            {
                continue;
            }

            // Redraw search UI
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, Console.CursorTop);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"(reverse-i-search)`{searchBuffer}': ");
            Console.ResetColor();
            
            if (matches.Count > 0)
            {
                Console.Write(matches[matchIndex]);
            }
        }
    }

    private void UpdateSearch(string searchTerm, out List<string> matches, out int matchIndex)
    {
        matches = new List<string>();
        
        // Search through history in reverse order
        for (int i = _history.Count - 1; i >= 0; i--)
        {
            var item = _history.GetAt(i);
            if (item != null && item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            {
                if (!matches.Contains(item))
                {
                    matches.Add(item);
                }
            }
        }

        matchIndex = 0;
    }
}
