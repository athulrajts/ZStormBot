using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using ZStormBot.Interfaces;

namespace ZStormBot.Services;

public class SoundRepository : ISoundRepository
{
    const string FILE = "Constants/sounds.json";

    private readonly Dictionary<string, SoundInfo> _soundsByName;
    private readonly Dictionary<int, SoundInfo> _soundsById;

    public SoundRepository()
    {
        var dirInfo = new DirectoryInfo("Sounds");
        _soundsByName = new Dictionary<string, SoundInfo>();
        int id = 0;
        foreach (var item in dirInfo.EnumerateFiles().OrderBy(x => x.LastWriteTimeUtc))
        {
            _soundsByName.Add(item.Name.Replace(item.Extension, string.Empty), new SoundInfo
            {
                Name = item.Name.Replace(item.Extension, string.Empty),
                Path = $"Sounds/{item.Name}",
                Id = id++,
            });
        }

        Save();

        _soundsByName = JsonSerializer.Deserialize<Dictionary<string, SoundInfo>>(File.ReadAllText(FILE), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        _soundsById = _soundsByName.ToDictionary(x => x.Value.Id, x => x.Value);
    }


    public async Task PlaySound(VoiceNextConnection vnc, string soundName)
    {
        var sound = _soundsByName.Values.FirstOrDefault(x => x.Name == soundName || x.Aliases.Contains(soundName));

        if (sound is null) return;

        await PlaySoundFile(vnc, sound.Path);
    }

    public async Task PlaySound(VoiceNextConnection vnc, int soundId)
    {
        if (!_soundsById.ContainsKey(soundId)) return;

        await PlaySoundFile(vnc, _soundsById[soundId].Path);
    }

    public IEnumerable<SoundInfo> GetAllSounds() => _soundsByName.Values.OrderBy(x => x.Id);

    public async Task AddSound(DiscordAttachment attachment)
    {
        var sound = new SoundInfo
        {
            Name = attachment.FileName,
            Path = $"Sounds/{attachment.FileName}",
            Id = _soundsByName.Values.LastOrDefault()?.Id + 1 ?? 1
        };

        using var client = new HttpClient();
        await client.DownloadFileTaskAsync(new Uri(attachment.Url), sound.Path);
        _soundsByName.Add(sound.Name, sound);
        _soundsById.Add(sound.Id, sound);
    }

    public void Save()
    {
        File.WriteAllText(FILE, JsonSerializer.Serialize(_soundsByName, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }));
    }

    public async Task PlaySoundFile(VoiceNextConnection vnc, string fileName)
    {
        await vnc.SendSpeakingAsync(true); // send a speaking indicator

        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $@"-i ""{fileName}"" -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        var ffmpeg = Process.Start(psi);
        var ffout = ffmpeg.StandardOutput.BaseStream;

        await ffout.CopyToAsync(vnc.GetTransmitSink());

        await ffout.DisposeAsync();

        await vnc.SendSpeakingAsync(false); // we're not speaking anymore
    }
}

public static class HttpClientUtils
{
    public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
    {
        using (var s = await client.GetStreamAsync(uri))
        {
            using (var fs = new FileStream(FileName, FileMode.CreateNew))
            {
                await s.CopyToAsync(fs);
            }
        }
    }
}
