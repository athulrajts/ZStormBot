using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace ZStormBot.Commands;

public partial class DotaCommands
{
    [Command("lgstats")]
    [Description("Gets last game stats of user")]
    public async Task LastGamePlayerStats(CommandContext ctx, int steamId)
    {
        await ctx.TriggerTypingAsync();

        var match = await OpenDota.GetLastGame(steamId);

        if (match is null) return;

        var heroName = Constants.GetHeroName(match.HeroId);
        var heroImage = Constants.GetHeroThumbnail(match.HeroId);

        var embed = new DiscordEmbedBuilder()
            .WithThumbnail(heroImage)
            .WithTitle(heroName)
            .WithUrl(match.GetDotaBuffUrl())
            .WithImageUrl(await SummaryImageRepository.LastGame(match))
            .Build();

        await ctx.RespondAsync(embed).ConfigureAwait(false);
    }

    [Command("lgstats")]
    [Description("Gets last game stats of user")]
    public async Task LastGamePlayerStats(CommandContext ctx, DiscordMember member)
    {
        TryGetSteamId(member, out int steamId);

        await LastGamePlayerStats(ctx, steamId);
    }


    [Command("lgstats")]
    [Description("Gets last game stats of user")]
    public async Task LastGamePlayerStats(CommandContext ctx)
    {
        await LastGamePlayerStats(ctx, ctx.Member);
    }

}
