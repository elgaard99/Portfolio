using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlLocalizationService
{
    private Dictionary<string, string> _translations = new();
    public string this[string key] => GetTranslation(key);
    
    private readonly string _webrootPath;
    private readonly string[] _supportedLanguages;
    private string _currentLanguage;
    private string _currentPage;

    public YamlLocalizationService(IWebHostEnvironment webHost, string[] supportedLanguages, string defaultLanguage = "en")
    {
        _supportedLanguages = supportedLanguages;
        _currentLanguage = supportedLanguages.Contains(defaultLanguage) ? defaultLanguage : throw new ArgumentException($"defaultLanguage \"{defaultLanguage}\" is not in supported languages.");
        _webrootPath = webHost.WebRootPath;
    }
    
    public async Task LoadTranslations(string page)
    {
        _currentPage = page;
        
        var path = Path.Combine(_webrootPath, "locales", page, $"{page}.{_currentLanguage}.yml");
        var yaml = await File.ReadAllTextAsync(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _translations = deserializer.Deserialize<Dictionary<string, string>>(yaml);
    }

    private string GetTranslation(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : key;
    }

    public async Task SetLanguage(string language)
    {
        _currentLanguage = language ?? "en";
        await LoadTranslations(_currentPage);
    }
}
