using System.Diagnostics;

namespace NShell.Shell.Readline;

/// <summary>
/// <c>KeyHandler</c> is a class that handle all about key input.
/// define different key handling methods
/// </summary>
public partial class KeyHandler
{
	public void HandleNormalChar()
	{
		_inputBuffer.Insert(_currentCursorPos4Input, _currentKey.KeyChar);
		_inputLength++;
		Console.Write((string?)_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input));
		_currentCursorPos4Input++;
		_currentCursorPos4Console++;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleBackspaceChar()
	{
		if (IsStartOfLine())
			return;
		_inputBuffer = _inputBuffer.Remove(_currentCursorPos4Input - 1, 1);
		_inputLength--;
		_currentCursorPos4Input--;
		_currentCursorPos4Console--;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		Console.Write(_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input) + " ");
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleDeleteChar()
	{
		if (IsEndOfLine())
			return;
		_inputBuffer.Remove(_currentCursorPos4Input, 1);
		_inputLength--;
		Console.Write(_inputBuffer.ToString(_currentCursorPos4Input, _inputLength - _currentCursorPos4Input) + " ");
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	public void HandleTab()
	{
		string currentInput = _inputBuffer.ToString();
		if (string.IsNullOrWhiteSpace(currentInput))
			return;

		var parts = currentInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 0)
			return;

		// If we're completing the first word (command name)
		if (parts.Length == 1 && !currentInput.EndsWith(' '))
		{
			var matches = GetCommandMatches(parts[0]);
			if (matches.Count == 1)
			{
				// Complete the command
				CompleteWith(matches[0]);
			}
			else if (matches.Count > 1)
			{
				// Show possible completions
				Console.WriteLine();
				foreach (var match in matches.Take(20))
				{
					Console.Write($"{match}  ");
				}
				if (matches.Count > 20)
					Console.Write($"... and {matches.Count - 20} more");
				Console.WriteLine();
				// Redraw prompt
				Console.Write(new string(' ', _initCursorPos4Console));
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(new string(' ', _initCursorPos4Console));
				Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
				Console.Write(_inputBuffer);
				_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			}
		}
		// If we're completing a file/directory path
		else
		{
			var lastPart = parts[^1];
			var matches = GetPathMatches(lastPart);
			
			if (matches.Count == 1)
			{
				// Remove the partial path from current input
				int removeLength = lastPart.Length;
				_inputBuffer.Remove(_inputLength - removeLength, removeLength);
				_inputLength -= removeLength;
				_currentCursorPos4Input -= removeLength;
				_currentCursorPos4Console -= removeLength;
				
				// Add the completed path
				CompleteWith(matches[0]);
			}
			else if (matches.Count > 1)
			{
				// Show possible completions
				Console.WriteLine();
				foreach (var match in matches.Take(20))
				{
					Console.Write($"{match}  ");
				}
				if (matches.Count > 20)
					Console.Write($"... and {matches.Count - 20} more");
				Console.WriteLine();
				// Redraw prompt
				Console.Write(new string(' ', _initCursorPos4Console));
				Console.SetCursorPosition(0, Console.CursorTop);
				Console.Write(new string(' ', _initCursorPos4Console));
				Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
				Console.Write(_inputBuffer);
				_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			}
		}
	}

	private List<string> GetCommandMatches(string prefix)
	{
		var matches = new List<string>();
		
		// Check custom commands
		foreach (var cmd in NShell.Shell.Commands.CommandParser.CustomCommands.Keys)
		{
			if (cmd.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				matches.Add(cmd);
		}
		
		// Check system commands
		foreach (var cmd in NShell.Shell.Commands.CommandParser.SystemCommands)
		{
			if (cmd.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				matches.Add(cmd);
		}
		
		return matches.OrderBy(x => x).ToList();
	}

	private List<string> GetPathMatches(string prefix)
	{
		var matches = new List<string>();
		
		try
		{
			string directory;
			string filePrefix;
			
			if (prefix.Contains('/'))
			{
				int lastSlash = prefix.LastIndexOf('/');
				directory = prefix.Substring(0, lastSlash + 1);
				filePrefix = prefix.Substring(lastSlash + 1);
				
				if (!Path.IsPathRooted(directory))
					directory = Path.Combine(Directory.GetCurrentDirectory(), directory);
			}
			else
			{
				directory = Directory.GetCurrentDirectory();
				filePrefix = prefix;
			}
			
			if (Directory.Exists(directory))
			{
				// Get matching files and directories
				foreach (var item in Directory.GetFileSystemEntries(directory))
				{
					var name = Path.GetFileName(item);
					if (name.StartsWith(filePrefix, StringComparison.OrdinalIgnoreCase))
					{
						// Add trailing slash for directories
						if (Directory.Exists(item))
							matches.Add(name + "/");
						else
							matches.Add(name);
					}
				}
			}
		}
		catch
		{
			// Ignore errors in path completion
		}
		
		return matches.OrderBy(x => x).ToList();
	}

	private void CompleteWith(string completion)
	{
		// Clear current input on screen
		Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
		Console.Write(new string(' ', _inputLength));
		Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
		
		// Replace input buffer
		_inputBuffer.Clear();
		_inputBuffer.Append(completion);
		_inputLength = completion.Length;
		_currentCursorPos4Input = _inputLength;
		_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
		
		// Write new input
		Console.Write(completion);
	}

	public void PreviousHistory()
	{
		var prev = _history.GetPrevious();
		if (prev != null)
		{
			Console.Write(new string('\b', _inputLength) + new string(' ', _inputLength) + new string('\b', _inputLength));
			_inputBuffer.Clear();
			_inputLength = prev.Length;
			_inputBuffer.Append(prev);
			Console.Write((object?)_inputBuffer);
			_currentCursorPos4Input = _inputLength;
			_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		}
	}

	public void NextHistory()
	{
		var next = _history.GetNext();
		if (next != null)
		{
			Console.Write(new string('\b', _inputLength) + new string(' ', _inputLength) + new string('\b', _inputLength));
			_inputBuffer.Clear();
			_inputLength = next.Length;
			_inputBuffer.Append(next);
			Console.Write((object?)_inputBuffer);
			_currentCursorPos4Input = _inputLength;
			_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		}
	}

	private void MoveCursorToEnd()
	{
		_currentCursorPos4Input = _inputLength;
		_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private void MoveCursorToStart()
	{
		_currentCursorPos4Input = 0;
		_currentCursorPos4Console = _initCursorPos4Console;
		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	private void MoveCursorWordLeft()
	{
		if (_currentCursorPos4Input == 0)
			return;

		// Skip whitespace
		while (_currentCursorPos4Input > 0 && char.IsWhiteSpace(_inputBuffer[_currentCursorPos4Input - 1]))
		{
			_currentCursorPos4Input--;
			_currentCursorPos4Console--;
		}

		// Move to start of word
		while (_currentCursorPos4Input > 0 && !char.IsWhiteSpace(_inputBuffer[_currentCursorPos4Input - 1]))
		{
			_currentCursorPos4Input--;
			_currentCursorPos4Console--;
		}

		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	private void MoveCursorWordRight()
	{
		if (_currentCursorPos4Input >= _inputLength)
			return;

		// Skip whitespace
		while (_currentCursorPos4Input < _inputLength && char.IsWhiteSpace(_inputBuffer[_currentCursorPos4Input]))
		{
			_currentCursorPos4Input++;
			_currentCursorPos4Console++;
		}

		// Move to end of word
		while (_currentCursorPos4Input < _inputLength && !char.IsWhiteSpace(_inputBuffer[_currentCursorPos4Input]))
		{
			_currentCursorPos4Input++;
			_currentCursorPos4Console++;
		}

		Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
	}

	private void HandleHistorySearch()
	{
		var search = new History.HistorySearch(_history);
		var result = search.Search(_initCursorPos4Console);
		
		if (result != null)
		{
			// Clear current input
			Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
			Console.Write(new string(' ', _inputLength));
			Console.SetCursorPosition(_initCursorPos4Console, Console.CursorTop);
			
			// Set new input
			_inputBuffer.Clear();
			_inputBuffer.Append(result);
			_inputLength = result.Length;
			_currentCursorPos4Input = _inputLength;
			_currentCursorPos4Console = _initCursorPos4Console + _inputLength;
			
			Console.Write(result);
		}
		else
		{
			// Restore cursor position if search was cancelled
			Console.SetCursorPosition(_currentCursorPos4Console, Console.CursorTop);
		}
	}

	private void MoveCursorLeft()
	{
		if (_currentCursorPos4Input == 0)
			return;
		_currentCursorPos4Console--;
		_currentCursorPos4Input--;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private void MoveCursorRight()
	{
		if (_currentCursorPos4Input == _inputLength)
			return;
		_currentCursorPos4Console++;
		_currentCursorPos4Input++;
		Console.SetCursorPosition(_currentCursorPos4Console,Console.CursorTop);
	}

	private bool IsEndOfLine() => _currentCursorPos4Input ==  _inputLength;

	private bool IsStartOfLine() => _currentCursorPos4Input == 0;
}
