using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OpenDotaApi;
using OpenDotaApi.Api.Matches.Model;
using ZStormBot.Constants;

namespace ZStormBot.Commands;

public partial class DotaCommands
{
    [Command("rg")]
    public async Task RecentGames(CommandContext ctx, int steamId)
    {
        await ctx.TriggerTypingAsync();

        var matches = await OpenDota.Players.GetRecentMatchesAsync(steamId);

        if (matches is null) return;

        var builder = new DiscordEmbedBuilder()
            .WithImageUrl(await SummaryImageRepository.RecentGames(matches.Take(10)));

        await ctx.RespondAsync(builder);
    }

    [Command("rg")]
    public async Task RecentGames(CommandContext ctx)
    {
        TryGetSteamId(ctx.Member, out int steamId);

        await RecentGames(ctx, steamId);
    }

    [Command("rgas")]
    public async Task RecentGamesAs(CommandContext ctx, int steamId, string alias)
    {
        await ctx.TriggerTypingAsync();

        if(!HeroNames.TryGetFromAlias(alias.ToLower(), out string heroName))
        {
            await ctx.RespondAsync($"hero - {alias} not found");
        }

        var heroId = Constants.GetHeroInfo(heroName).Id;

        var playerMatches = await OpenDota.Players.GetMatchesAsync(steamId, new PlayerParameters { HeroId = heroId });
        var matchPlayerers = new List<MatchPlayer>();
        foreach (var match in playerMatches.Take(10))
        {
            if (match.MatchId.HasValue)
            {
                try
                {
                    var matchModel = await OpenDota.Matches.GetMatchAsync(match.MatchId.Value);
                    var player = matchModel.Players.FirstOrDefault(x => x.AccountId == steamId);
                    matchPlayerers.Add(player);
                }
                catch { }
            }
        }

        var builder = new DiscordEmbedBuilder()
            .WithThumbnail(Constants.GetHeroThumbnail(heroId))
            .WithImageUrl(await SummaryImageRepository.RecentGamesAs(matchPlayerers));

        await ctx.RespondAsync(builder).ConfigureAwait(false);
    }

    [Command("rgas")]
    public async Task RecentGamesAs(CommandContext ctx, DiscordMember member, string alias)
    {
        TryGetSteamId(member, out int steamId);
        await RecentGamesAs(ctx, steamId, alias);
    }

    [Command("rgas")]
    public async Task RecentGamesAs(CommandContext ctx, string alias)
    {
        await RecentGamesAs(ctx, ctx.Member, alias);
    }



    [Command("rgag")]
    public async Task RecentGamesAgainst(CommandContext ctx, int steamId, params string[] alias)
    {
        await ctx.TriggerTypingAsync();

        var heroes = new List<HeroInfo>();

        foreach (var item in alias)
        {
            if (!HeroNames.TryGetFromAlias(item.ToLower(), out string heroName))
            {
                await ctx.RespondAsync($"hero - {item} not found");
                return;
            }
            heroes.Add(Constants.GetHeroInfo(heroName));
        }

        var playerMatches = await OpenDota.Players.GetMatchesAsync(steamId, new PlayerParameters { AgainstHeroId = heroes.Select(x => x.Id)});
        var matchPlayerers = new List<MatchPlayer>();
        foreach (var match in playerMatches.Take(10))
        {
            var matchModel = await OpenDota.Matches.GetMatchAsync(match.MatchId.Value);
            var player = matchModel.Players.FirstOrDefault(x => x.AccountId == steamId);
            matchPlayerers.Add(player);
        }

        var builder = new DiscordEmbedBuilder()
            .WithTitle($"Matches Against {string.Join(",", heroes.Select(x => x.LocalizedName))}")
            .WithImageUrl(await SummaryImageRepository.RecentGamesWithOrAgainst(matchPlayerers));

        await ctx.RespondAsync(builder).ConfigureAwait(false);
    }

    [Command("rgag")]
    public async Task RecentGamesAgainst(CommandContext ctx, params string[] alias)
    {
        TryGetSteamId(ctx.Member, out int steamId);

        await RecentGamesAgainst(ctx, steamId, alias);
    }

    [Command("rgw")]
    public async Task RecentGamesWith(CommandContext ctx, int steamId, params string[] alias)
    {
        await ctx.TriggerTypingAsync();

        var heroes = new List<HeroInfo>();

        foreach (var item in alias)
        {
            if (!HeroNames.TryGetFromAlias(item.ToLower(), out string heroName))
            {
                await ctx.RespondAsync($"hero - {item} not found");
                return;
            }
            heroes.Add(Constants.GetHeroInfo(heroName));
        }

        var playerMatches = await OpenDota.Players.GetMatchesAsync(steamId, new PlayerParameters { WithHeroId = heroes.Select(x => x.Id) });
        var matchPlayerers = new List<MatchPlayer>();
        foreach (var match in playerMatches.Take(10))
        {
            var matchModel = await OpenDota.Matches.GetMatchAsync(match.MatchId.Value);
            var player = matchModel.Players.FirstOrDefault(x => x.AccountId == steamId);
            matchPlayerers.Add(player);
        }

        var builder = new DiscordEmbedBuilder()
            .WithTitle($"Matches with {string.Join(",", heroes.Select(x => x.LocalizedName))}")
            .WithImageUrl(await SummaryImageRepository.RecentGamesWithOrAgainst(matchPlayerers));

        await ctx.RespondAsync(builder).ConfigureAwait(false);
    }

    [Command("rgw")]
    public async Task RecentGamesWith(CommandContext ctx, params string[] alias)
    {
        TryGetSteamId(ctx.Member, out int steamId);

        await RecentGamesWith(ctx, steamId, alias);
    }
}
