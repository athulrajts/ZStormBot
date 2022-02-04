using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace ZStormBot.Commands;

public partial class DotaCommands
{
    [Command("itemusage")]
    public async Task ItemsUsed(CommandContext ctx, long matchId, int steamId)
    {
        await ctx.TriggerTypingAsync();

        var match = await OpenDota.Matches.GetMatchAsync(matchId);
        var player = match.Players.FirstOrDefault(x => x.AccountId == steamId);

        var builder = new DiscordEmbedBuilder()
            .WithThumbnail(Constants.GetHeroThumbnail(player.HeroId))
            .WithTitle(player.Personaname);

        if (player.ItemUses is { })
        {
            foreach (var item in player.ItemUses)
            {
                builder.AddField(item.Key, item.Value.ToString());
            }
        }

        await ctx.RespondAsync(builder).ConfigureAwait(false);
    }

    [Command("itemusage")]
    public async Task ItemsUsed(CommandContext ctx, long matchId)
    {
        await ItemsUsed(ctx, ctx.Member, matchId);
    }

    [Command("itemusage")]
    public async Task ItemsUsed(CommandContext ctx, DiscordMember member, long matchId)
    {
        TryGetSteamId(member, out int steamId);

        await ItemsUsed(ctx, matchId, steamId);
    }

    [Command("itemusage")]
    public async Task ItemsUsed(CommandContext ctx)
    {
        await ItemsUsed(ctx, ctx.Member);
    }

    [Command("itemusage")]
    public async Task ItemsUsed(CommandContext ctx, DiscordMember member)
    {
        TryGetSteamId(member, out int steamId);

        var match = await OpenDota.GetLastGame(steamId);

        if (match is null) return;

        await ItemsUsed(ctx, match.MatchId.Value, steamId);
    }
}
