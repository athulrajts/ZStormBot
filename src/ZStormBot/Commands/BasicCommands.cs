using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ZStormBot.Interfaces;
using DSharpPlus.VoiceNext;

namespace ZStormBot.Commands;

public class BasicCommands : BaseCommandModule 
{
    public IUserRepository UserRepository { get; set; }

    [Command("ping")]
    [Description("Example ping command")]
    [Aliases("pong")]
    public async Task Ping(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

        await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
    }

    [Command("setSteamId")]
    public Task SetSteamId(CommandContext ctx, int steamId)
    {
        UserRepository.SetSteamId(ctx.Member.Discriminator, steamId);

        ctx.RespondAsync($"User {ctx.Member.DisplayName}({steamId}) registered");

        return Task.CompletedTask;
    }


    [Command("setSteamId")]
    public Task SetSteamId(CommandContext ctx, DiscordMember member, int steamId)
    {
        UserRepository.SetSteamId(member.Discriminator, steamId);

        ctx.RespondAsync($"User {member.DisplayName}({steamId}) registered");

        return Task.CompletedTask;
    }

    [Command("invite")]
    public async Task JoinVoiceChannel(CommandContext ctx)
    {
        var vnext = ctx.Client.GetVoiceNext();

        var vnc = vnext.GetConnection(ctx.Guild);
        if (vnc != null)
            throw new InvalidOperationException("Already connected in this guild.");

        var chn = ctx.Member?.VoiceState?.Channel;
        if (chn == null)
            throw new InvalidOperationException("You need to be in a voice channel.");

        vnc = await vnext.ConnectAsync(chn);

    }

    [Command("exit")]
    public async Task ExitVoiceChannel(CommandContext ctx)
    {
        var vnext = ctx.Client.GetVoiceNext();

        var vnc = vnext.GetConnection(ctx.Guild);
        if (vnc == null)
        {
            throw new InvalidOperationException("Not connected in this guild.");
        }

        vnc.Disconnect();
        await Task.Delay(0);
    }

    [Command("inviteurl")]
    public async Task InviteUrl(CommandContext ctx)
    {
        await ctx.RespondAsync("https://discord.com/oauth2/authorize?client_id=935785059535978507&scope=bot&permissions=8");
    }
}
