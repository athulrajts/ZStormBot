using ZStormBot.Constants;

namespace ZStormBot.Interfaces;

public interface IDotaConstants
{
    HeroInfo GetHeroInfo(int id);
    HeroInfo GetHeroInfo(string name);

    string GetHeroName(int? id);
    string GetHeroName(string name);

    string GetHeroThumbnail(int? id);
    string GetHeroThumbnail(string name);

    string GetHeroIcon(int? id);
    string GetHeroIcon(string name);

    PatchInfo GetPatch(string id);
    PatchInfo GetLastPatch();
}
