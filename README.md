
![NShellBanner](https://github.com/user-attachments/assets/f4feb3d9-3105-459f-b9da-c37df1b67446)

> A custom C# interactive shell, customizable.

![NShell Preview](https://github.com/user-attachments/assets/1635d16b-c6b1-4379-8ee6-2a0d5c8161fb)


---

### 🧠 What is NShell?

**NShell** is a lightweight, extensible shell written in C# using [.NET 8.0+].  
It's designed for hackers, shell lovers, and those who enjoy boot sequences.

---

### ✨ Features

- ✅ Custom interactive shell interface
- ✅ Built-in command loader with registration system
- ✅ Command metadata (`Description`, `IsInteractive`, `RequiresRoot`, etc.)
- ✅ Handle customs commands, but load also `/usr/bin`, `/usr/sbin`..ect commands.
- ✅ Spectre.Console markup support for colors, glitches, animations
- ✅ Full AOT support (with manual command registration)
- ✅ Future-proof extensibility (plugin-style architecture)
- ✅ Tab completion for commands and file paths
- ✅ Command history with Ctrl+R reverse search
- ✅ Command aliases (alias/unalias)
- ✅ Environment variable management (export/unset/printenv)
- ✅ Word-based cursor navigation (Ctrl+Left/Right)
- ✅ Rich built-in commands (help, history, which, pwd, echo, clear, exit)


---

### 🚀 Installation

Clone the repo and install:

```bash
git clone https://github.com/onihilist/NShell.git
cd NShell
chmod +x install.sh
./install.sh
```

---

### 🎨 Custom themes

This is a little exemple of an custom theme.</br>
If you are using `format_top/bottom` & `corner_top/bottom`, `format` will be ignored.</br>
For making a single line prompt use `format`, and double line `format_top/bottom`.</br>
N.B : `path_slash_color` & `path_words_color` works with single and double line shell prompt.
Exemple :
```json
{
  "name": "test",
  "format": "[bold green][[{user}@{host}]][/][white][[{cwd}]] >>[/]",
  "format_top": "[[[bold cyan]{user}[/]@[bold cyan]{host}[/]]]",
  "format_bottom": "[[{cwd}]]",
  "corner_top": "[bold magenta]\u250c[/]",
  "corner_bottom": "[bold magenta]\u2514[/]",
  "ls_colors": "di=34:fi=37:ln=36:pi=33:so=35:ex=32",
  "path_slash_color": "cyan",
  "path_words_color": "yellow"
}
```

The name of the theme is `test`, no matter what the file is named.</br>
So enter the command : `settheme test`.</br>
This is the result :

![Preview Test Theme](https://github.com/user-attachments/assets/eaea69b7-0f9e-4f0f-8d0b-7c92ffe8c4f1)

---

### 📡 Roadmap v1.0.0

- [PROGRESS] Plugin support (dynamic loading)
- [OK] Fix neofetch shell version
- [OK] Fix interactive commands/scripts running configuration
- [OK] Autocomplete
- [OK] Command history
- [OK] Profiles and theme switching
- [OK] Remove Bash FallBack
- [OK] Themes & ThemeLoader

---

### 🔨 Built-in Commands

NShell comes with a rich set of built-in commands:

| Command | Description |
|---------|-------------|
| `help` | Display all available commands with descriptions |
| `exit` | Exit the shell gracefully |
| `clear` | Clear the terminal screen |
| `cd <dir>` | Change directory |
| `pwd` | Print current working directory |
| `echo <text>` | Display text (supports variable expansion) |
| `history [n]` | Show command history (optionally last n commands) |
| `alias name='cmd'` | Create command alias |
| `unalias name` | Remove command alias |
| `export VAR=value` | Set environment variable |
| `unset VAR` | Remove environment variable |
| `printenv [VAR]` | Print environment variables |
| `which <cmd>` | Show path to command |
| `settheme <name>` | Change shell theme |

**Keyboard Shortcuts:**
- `Tab` - Auto-complete commands and paths
- `Ctrl+R` - Reverse history search
- `Ctrl+A` / `Home` - Move cursor to start of line
- `Ctrl+E` / `End` - Move cursor to end of line
- `Ctrl+Left/Right` - Move cursor by word
- `Up/Down` - Navigate command history

---

### 🔧 Troubleshooting

If you have any problem with **NShell**, or it locks you out of a proper shell,  
you can forcefully switch back to `bash` like this:

```bash
sudo sed -i "s|/usr/local/bin/nshell|/bin/bash|" /etc/passwd
```

If you got the error "bad interpreter" when running `install.sh` try to run this :

```bash
sudo apt update
sudo apt install dos2unix
dos2unix install.sh
```


