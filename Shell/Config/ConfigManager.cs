using System.Text.Json;

namespace NShell.Shell.Config;

/// <summary>
/// Manages saving and loading shell configuration including aliases.
/// </summary>
public class ConfigManager
{
    private readonly string _configPath;
    
    public ConfigManager()
    {
        var configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nshell"
        );
        
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
        
        _configPath = Path.Combine(configDir, "nshellrc.json");
    }

    /// <summary>
    /// Save aliases to configuration file.
    /// </summary>
    public void SaveAliases(Dictionary<string, string> aliases)
    {
        try
        {
            var config = LoadConfig();
            config["aliases"] = aliases;
            
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(_configPath, json);
        }
        catch (Exception)
        {
            // Silently fail - don't interrupt shell operation
        }
    }

    /// <summary>
    /// Load aliases from configuration file.
    /// </summary>
    public Dictionary<string, string> LoadAliases()
    {
        try
        {
            var config = LoadConfig();
            
            if (config.ContainsKey("aliases") && config["aliases"] is JsonElement aliasesElement)
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(aliasesElement.ToString()) 
                    ?? new Dictionary<string, string>();
            }
        }
        catch (Exception)
        {
            // Return empty dictionary on error
        }
        
        return new Dictionary<string, string>();
    }

    /// <summary>
    /// Load entire configuration file.
    /// </summary>
    private Dictionary<string, object> LoadConfig()
    {
        if (!File.Exists(_configPath))
        {
            return new Dictionary<string, object>();
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) 
                ?? new Dictionary<string, object>();
        }
        catch (Exception)
        {
            return new Dictionary<string, object>();
        }
    }
}
