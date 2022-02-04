using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace ZStormBot.Interfaces;

public interface ISoundRepository
{
    Task PlaySound(VoiceNextConnection vnc, string soundName);
    Task PlaySound(VoiceNextConnection vnc, int soundId);
    IEnumerable<SoundInfo> GetAllSounds();
    Task AddSound(DiscordAttachment sound);
    void Save();
}

public class SoundInfo
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Path { get; set; }
    public List<string> Aliases { get; set; } = new();
}