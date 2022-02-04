using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace ZStormBot.Commands;

public partial class DotaCommands
{
    [Command("stunduration")]
    public async Task StunDuration(CommandContext ctx, long matchId, int steamId)
    {
        await ctx.TriggerTypingAsync();

        var match = await OpenDota.Matches.GetMatchAsync(matchId);
        var player = match.Players.FirstOrDefault(x => x.AccountId == steamId);

        var builder = new DiscordEmbedBuilder()
            .WithThumbnail(Constants.GetHeroThumbnail(player.HeroId))
            .WithTitle(player.Personaname)
            .WithDescription($"Stun Duration : {player.Stuns:N2}");

        await ctx.RespondAsync(builder).ConfigureAwait(false);
    }

    [Command("stunduration")]
    public async Task StunDuration(CommandContext ctx, long matchId)
    {
        if (UserRepository.TryGetSteamId(ctx.Member.Discriminator, out int steamId) == false)
        {
            await ctx.RespondAsync($"SteamId for {ctx.Member.DisplayName} not registered");
            return;
        }

        await StunDuration(ctx, matchId, steamId);
    }

    [Command("stunduration")]
    public async Task StunDuration(CommandContext ctx)
    {
        if (UserRepository.TryGetSteamId(ctx.Member.Discriminator, out int steamId) == false)
        {
            await ctx.RespondAsync($"SteamId for {ctx.Member.DisplayName} not registered");
            return;
        }

        var match = await OpenDota.GetLastGame(steamId);

        if (match is null) return;

        await StunDuration(ctx, match.MatchId.Value, steamId);
    }
}
