using OpenDotaApi;
using OpenDotaApi.Api.Heroes.Model;
using OpenDotaApi.Api.Matches.Model;
using OpenDotaApi.Api.Players.Model.RecentMatches;

namespace ZStormBot;

internal static class DotaApi
{
    private static OpenDota client = new OpenDota();
    private static List<Hero> heroes = new();
    
    public static async Task Initialize()
    {
        heroes = await client.Heroes.GetDataAsync();
    }

    public static async Task<RecentMatches> GetLastGame(int steamId)
    {
        var player = await client.Players.GetPlayerAsync(steamId);
        var matches = await client.Players.GetRecentMatchesAsync(player.Profile.AccountId);
        return matches.FirstOrDefault();
    }

    public static async Task<Match> GetLastGameFull(int steamId)
    {
        var player = await client.Players.GetPlayerAsync(steamId);
        var matches = await client.Players.GetRecentMatchesAsync(player.Profile.AccountId);
        var match = matches.FirstOrDefault();

        return await client.Matches.GetMatchAsync(match.MatchId.Value);
    }

    public static Hero GetHeroById(int id) => heroes.FirstOrDefault(x => x.Id == id);

    public static string GetHeroThumbnail(int id)
    {
        return $"https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/{heroes.FirstOrDefault(x => x.Id == id)?.LocalizedName.Replace(" ", "_").ToLower()}.png";
    }
}
