using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using OpenDotaApi;
using ZStormBot.Interfaces;

namespace ZStormBot.Commands;

public partial class DotaCommands : BaseCommandModule
{
    public OpenDota OpenDota { get; set; }
    public IDotaConstants Constants { get; set; }
    public ISummaryImageRepository SummaryImageRepository { get; set; }
    public IUserRepository UserRepository { get; set; }

    private void TryGetSteamId(DiscordMember member, out int steamId)
    {
        if (UserRepository.TryGetSteamId(member.Discriminator, out steamId) == false)
        {
            throw new ArgumentException($"SteamId for {member.DisplayName}not registered", nameof(member));
        }
    }

    private void TryGetSteamId(DiscordMember[] members, out IList<int> steamIds)
    {
        steamIds = new List<int>();

        foreach (var member in members)
        {
            TryGetSteamId(member, out int id);
            steamIds.Add(id);
        }
    }
}
