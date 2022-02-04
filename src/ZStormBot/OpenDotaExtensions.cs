using OpenDotaApi;
using OpenDotaApi.Api.Matches.Model;
using OpenDotaApi.Api.Players.Model.RecentMatches;

namespace ZStormBot
{
    public static class OpenDotaExtensions
    {
        public static async Task<RecentMatches> GetLastGame(this OpenDota client, int steamId)
        {
            var player = await client.Players.GetPlayerAsync(steamId);
            var matches = await client.Players.GetRecentMatchesAsync(player.Profile.AccountId);
            return matches.FirstOrDefault();
        }

        public static async Task<(Match, RecentMatches)> GetLastGameFull(this OpenDota client, int steamId)
        {
            var player = await client.Players.GetPlayerAsync(steamId);
            var matches = await client.Players.GetRecentMatchesAsync(player.Profile.AccountId);
            var match = matches.FirstOrDefault();

            return new (await client.Matches.GetMatchAsync(match.MatchId.Value), match);
        }

        public static string GetDotaBuffUrl(this RecentMatches match) => $"https://www.dotabuff.com/matches/{match.MatchId}";
        public static string GetDotaBuffUrl(this Match match) => $"https://www.dotabuff.com/matches/{match.MatchId}";
    }
}
