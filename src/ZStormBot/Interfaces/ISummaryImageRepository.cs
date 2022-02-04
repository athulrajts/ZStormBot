using OpenDotaApi.Api.Matches.Model;
using OpenDotaApi.Api.Players.Model.RecentMatches;
using PlayerMatch = OpenDotaApi.Api.Players.Model.Matches.Matches;

namespace ZStormBot.Interfaces;

public interface ISummaryImageRepository
{
    Task<string> LastGame(RecentMatches match);
    Task<string> LastGameFull(Match match);
    Task<string> RecentGames(IEnumerable<RecentMatches> match);
    Task<string> RecentGamesAs(IEnumerable<MatchPlayer> player);
    Task<string> RecentGamesWithOrAgainst(IEnumerable<MatchPlayer> player);
}
