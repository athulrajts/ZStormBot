using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text;
using ZStormBot.Constants;

namespace ZStormBot.Commands;

public partial class DotaCommands
{
    [Command("pnh")]
    public async Task PatchNotesForHero(CommandContext ctx, string heroAlias)
    {
        await ctx.TriggerTypingAsync();

        if (!HeroNames.TryGetFromAlias(heroAlias.ToLower(), out string heroName))
        {
            await ctx.RespondAsync($"hero - {heroAlias} not found");
        }

        var hero = Constants.GetHeroInfo(heroName);
        var patch = Constants.GetLastPatch();
        var key = hero.Name.Replace("npc_dota_hero_", string.Empty);

        if (patch.Heroes.ContainsKey(key))
        {
            var description = string.Join(Environment.NewLine, patch.Heroes[key].Select(x => $"- {x}"));

            var builder = new DiscordEmbedBuilder()
                .WithTitle($"{hero.LocalizedName}({patch.Name})")
                .WithDescription(description)
                .WithThumbnail(Constants.GetHeroThumbnail(hero.Id));

            await ctx.RespondAsync(builder).ConfigureAwait(false);

        }
        else
        {
            await ctx.RespondAsync("no changes");
        }
    }

    [Command("hpn")]
    public async Task PathNotesForHeroes(CommandContext ctx)
    {
        var patch = Constants.GetLastPatch();

        await PathNotesForHeroes(ctx, patch.Name);
    }

    [Command("hpn")]
    public async Task PathNotesForHeroes(CommandContext ctx, string patch)
    {
        async Task Respond(CommandContext ctx, StringBuilder sb, string patchName, int count)
        {
            var builder = new DiscordEmbedBuilder()
                .WithTitle(patchName + (count == 0 ? "" : $"({count})"))
                .WithDescription(sb.ToString());

            await ctx.RespondAsync(builder);

            sb.Clear();
        }

        await ctx.TriggerTypingAsync();
        var patchInfo = Constants.GetPatch(patch);

        var sb = new StringBuilder();
        int count = 0;

        foreach (var hero in patchInfo.Heroes)
        {
            if (sb.Length > 3900)
            {
                await Respond(ctx, sb, patch, count);
                count++;
            }

            var info = Constants.GetHeroInfo($"npc_dota_hero_{hero.Key}");
            sb.AppendLine($"**{info.LocalizedName}**");
            foreach (var description in hero.Value)
            {
                sb.AppendLine($"- {description}");
            }
        }

        if (sb.Length > 0)
        {
            await Respond(ctx, sb, patch, count);
        }
    }
}
