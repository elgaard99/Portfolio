using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Portfolio.Services;

public class YamlLocalizationService
{
    private Dictionary<string, string> _translations = new();
    private Dictionary<string, Dictionary<string, string>> _systemTranslations = new();
    public Dictionary<string, string> SystemTranslations(string section)
    {
        try
        {
            return _systemTranslations[section];
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e.Message);
        }

        return new Dictionary<string, string>();
    }

    public string this[string key] 
        => _translations.TryGetValue(key, out var value) ? value : key;

    private readonly string _webrootPath;
    private readonly string[] _supportedLanguages;
    public string CurrentLanguage { get; private set; }
    private string _currentPage;
    private ILogger<YamlLocalizationService> _logger;

    public event Action? LanguageChanged;

    public YamlLocalizationService(
        IWebHostEnvironment webHost,
        ILogger<YamlLocalizationService> logger,
        string[] supportedLanguages,
        string defaultLanguage = "en")
    {
        _supportedLanguages = supportedLanguages;
        CurrentLanguage = 
            supportedLanguages.Contains(defaultLanguage) ? defaultLanguage : throw new ArgumentException
            (
                $"defaultLanguage \"{defaultLanguage}\" is not in supported languages."
            );
        _webrootPath = webHost.WebRootPath;
        _logger = logger;
    }

    public async Task LoadTranslations(string page)
    {
        _currentPage = page;
        
        var path = Path.Combine(_webrootPath, "locales", page, $"{page}.{CurrentLanguage}.yml");
        _logger?.LogInformation($"Loading localizations from {path}");
        
        if (!File.Exists(path))
        {
            _logger?.LogError($"File {path} does not exist");
            _translations = new Dictionary<string, string>();
            LanguageChanged?.Invoke();
            return;
        }

        var yaml = await File.ReadAllTextAsync(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _translations = deserializer.Deserialize<Dictionary<string, string>>(yaml);
        foreach (var (key, value) in _translations)
            _logger?.LogDebug($"Translated {key} to {value}");
        
        // 🔹 Fire event after loading
        LanguageChanged?.Invoke();
    }

    public async Task LoadSystemTranslations()
    {
        var path = Path.Combine(_webrootPath, "locales", $"System.{CurrentLanguage}.yml");
        _logger?.LogInformation($"Loading localizations from {path}");
        
        if (!File.Exists(path))
        {
            _logger?.LogError($"File {path} does not exist");
            _translations = new Dictionary<string, string>();
            LanguageChanged?.Invoke();
            return;
        }

        var yaml = await File.ReadAllTextAsync(path);
        _logger.LogInformation($"Loaded data: {yaml}");
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Deserialize the YAML into a nested dictionary
        _systemTranslations = deserializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(yaml);
        foreach (var (key, value) in _systemTranslations)
            _logger?.LogDebug($"Translated {key} to {value}");
        
        
        // 🔹 Fire event after loading
        LanguageChanged?.Invoke();
    }

    public async Task SetLanguage(string language)
    {
        CurrentLanguage = _supportedLanguages.Contains(language) ? language : throw new ArgumentException
            (
                $"language \"{language}\" is not supported."
            );
        
        await LoadTranslations(_currentPage);
        await LoadSystemTranslations();
        LanguageChanged?.Invoke();
    }
}
