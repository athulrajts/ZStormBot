using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using ZStormBot.Interfaces;

namespace ZStormBot.Commands;

public class Sounds : BaseCommandModule
{
    public ISoundRepository SoundRepository { get; set; }

    [Command("cl")]
    public async Task PlaySound(CommandContext ctx, string name)
    {
        var vnext = ctx.Client.GetVoiceNext();
        var vnc = vnext.GetConnection(ctx.Guild);
        if (vnc is null)
        {
            await ctx.RespondAsync("Not connected in this guild.");
        }

        if (vnc.IsPlaying)
        {
            await ctx.RespondAsync("Voice channel already playing a sound, please wait");
            return;
        }

        if(int.TryParse(name, out int id))
        {
            await PlaySoundById(vnc, id);
        }
        else
        {
            await PlaySoundByName(vnc, name);
        }
    }

    public async Task PlaySoundById(VoiceNextConnection vnc, int id) => await SoundRepository.PlaySound(vnc, id);

    public async Task PlaySoundByName(VoiceNextConnection vnc, string name) => await SoundRepository.PlaySound(vnc, name);


    [Command("vlist")]
    public async Task ListVoiceLines(CommandContext ctx)
    {
        await ctx.TriggerTypingAsync();

        var sb = new StringBuilder();
        foreach (var sound in SoundRepository.GetAllSounds())
        {
            if(sb.Length > 1500)
            {
                await ctx.RespondAsync(sb.ToString());
                sb.Clear();
            }

            sb.AppendLine($"{sound.Id}. {sound.Name}");
        }

        if(sb.Length > 0)
        {
            await ctx.RespondAsync(sb.ToString());
        }
    }

    [Command("add_sound")]
    public async Task AddSound(CommandContext ctx)
    {
        if (ctx.Message.Attachments is null) return;
        if (ctx.Message.Attachments.Count == 0) return;

        var count = 0;
        foreach (var item in ctx.Message.Attachments)
        {
            if(!item.FileName.EndsWith("mp3"))
            {
                continue;
            }

            await SoundRepository.AddSound(item);
            await ctx.RespondAsync($"added {item.FileName}");
            count++;
        }

        if(count > 0)
        {
            SoundRepository.Save();
        }
    }


}
