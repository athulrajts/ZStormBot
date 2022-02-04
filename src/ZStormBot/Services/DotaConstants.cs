using System.Text;
using System.Text.Json;
using ZStormBot.Constants;
using ZStormBot.Interfaces;

namespace ZStormBot.Services;

public class DotaConstants : IDotaConstants
{
    const string FILE_HEROES = "Constants/heroes.json";
    const string FILE_ITEMS = "Constants/items.json";
    const string FILE_PATCH_NOTES = "Constants/patchnotes.json";
    const string BASE_URL = "https://cdn.cloudflare.steamstatic.com";

    private Dictionary<string, HeroInfo> _heroesByName;
    private Dictionary<int, HeroInfo> _heroesById;
    private Dictionary<string, ItemInfo> _itemsByName;
    private Dictionary<int, ItemInfo> _itemsById;
    private Dictionary<string, PatchInfo> _patchByName;

    public DotaConstants()
    {
        ReadConstants();
    }

    public string GetHeroName(int? id) => id.HasValue ? _heroesById[id.Value].LocalizedName : string.Empty;
    public string GetHeroName(string name) => _heroesByName[name].LocalizedName;

    public string GetHeroThumbnail(int? id) => id.HasValue ? $"{BASE_URL}{_heroesById[id.Value].ThumbnailImage}" : string.Empty;
    public string GetHeroThumbnail(string name) => $"{BASE_URL}{_heroesByName[name].ThumbnailImage}";

    public string GetHeroIcon(int? id) => id.HasValue ? $"{BASE_URL}{_heroesById[id.Value].Icon}" : string.Empty;
    public string GetHeroIcon(string name) => $"{BASE_URL}{_heroesByName[name].Icon}";

    public HeroInfo GetHeroInfo(int id) => _heroesById[id];
    public HeroInfo GetHeroInfo(string name) => _heroesByName[name];

    public PatchInfo GetPatch(string patch) => _patchByName[patch.Replace(".", "_")];
    public PatchInfo GetLastPatch() => _patchByName.Values.Last();

    private void ReadConstants()
    {
        ReadPatchNotes();
        ReadHeroes();
        ReadItems();
    }

    private void ReadPatchNotes()
    {
        _patchByName = JsonSerializer.Deserialize<Dictionary<string, PatchInfo>>(File.ReadAllText(FILE_PATCH_NOTES));

        foreach (var item in _patchByName)
        {
            item.Value.Name = item.Key.Replace("_", ".");
        }
    }

    private void ReadItems()
    {
        _itemsByName = JsonSerializer.Deserialize<Dictionary<string, ItemInfo>>(File.ReadAllText(FILE_ITEMS));
        _itemsById = _itemsByName.ToDictionary(x => x.Value.Id, x => x.Value);
    }

    private void ReadHeroes()
    {
        _heroesById = JsonSerializer.Deserialize<Dictionary<int, HeroInfo>>(File.ReadAllText(FILE_HEROES));
        _heroesByName = _heroesById.ToDictionary(x => x.Value.Name, x=> x.Value);


        var sb = new StringBuilder();
        sb.AppendLine("public class HeroNames");
        sb.AppendLine("{");
        foreach (var item in _heroesByName.OrderBy(x => x.Value.LocalizedName))
        {
            sb.AppendLine($"public const string {item.Value.LocalizedName.Replace(" ", string.Empty)} = \"{item.Value.Name}\"");
        }
        sb.AppendLine("}");
        File.WriteAllText("heronames.cs", sb.ToString());
    }
}
