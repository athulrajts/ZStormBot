using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OpenDotaApi;

namespace ZStormBot.Commands;

public partial class DotaCommands
{

    [Command("lg")]
    [Description("Gets last game stats")]
    public async Task LastGame(CommandContext ctx, int steamId)
    {
        await ctx.TriggerTypingAsync();

        var (fullMatch, match) = await OpenDota.GetLastGameFull(steamId);

        if (match is null) return;

        var heroName = Constants.GetHeroName(match.HeroId);
        var heroImage = Constants.GetHeroThumbnail(match.HeroId);

        var embed = new DiscordEmbedBuilder()
            .WithThumbnail(heroImage)
            .WithTitle(heroName)
            .WithUrl(match.GetDotaBuffUrl())
            .WithImageUrl(await SummaryImageRepository.LastGameFull(fullMatch))
            .Build();

        await ctx.RespondAsync(embed).ConfigureAwait(false);
    }

    [Command("lg")]
    [Description("Gets last game stats")]
    public async Task LastGame(CommandContext ctx, DiscordMember member)
    {
        TryGetSteamId(member, out int steamId);

        await LastGame(ctx, steamId);
    }

    [Command("lg")]
    [Description("Gets last game stats")]
    public async Task LastGame(CommandContext ctx)
    {
        await LastGame(ctx, ctx.Member);
    }

    [Command("lgwp")]
    public async Task LastGameWithPlayer(CommandContext ctx, int steamId, params int[] steamIdOther)
    {
        await ctx.TriggerTypingAsync();

        var matches = await OpenDota.Players.GetMatchesAsync(steamId, new PlayerParameters { IncludedAccountId = steamIdOther });
        var lastMatch = matches.FirstOrDefault();

        if (lastMatch is null)
        {
            await ctx.RespondAsync("no matches found");
        }

        var match = await OpenDota.Matches.GetMatchAsync(lastMatch.MatchId.Value);

        var heroName = Constants.GetHeroName(lastMatch.HeroId);
        var heroImage = Constants.GetHeroThumbnail(lastMatch.HeroId);

        var embed = new DiscordEmbedBuilder()
            .WithUrl(match.GetDotaBuffUrl())
            .WithThumbnail(heroImage)
            .WithTitle(heroName)
            .WithImageUrl(await SummaryImageRepository.LastGameFull(match))
            .Build();

        await ctx.RespondAsync(embed).ConfigureAwait(false);
    }

    [Command("lgwp")]
    public async Task LastGameWithPlayer(CommandContext ctx, params DiscordMember[] others)
    {
        TryGetSteamId(ctx.Member, out int steamId);
        TryGetSteamId(others, out IList<int> othersIds);

        await LastGameWithPlayer(ctx, steamId, othersIds.ToArray());
    }
}
