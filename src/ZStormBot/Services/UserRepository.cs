using System.Text.Json;
using ZStormBot.Interfaces;

namespace ZStormBot.Services;

public class UserRepository : IUserRepository
{
    const string FILE = "Config/users.json";
    private Dictionary<string, UserInfo> _users;

    public UserRepository()
    {
        Directory.CreateDirectory("Config");
        ReadConfig();
    }



    public void SetSteamId(string user, int steamId)
    {
        if(_users.ContainsKey(user))
        {
            _users[user].SteamId = steamId;
        }
        else
        {
            _users.Add(user, new UserInfo { SteamId = steamId });
        }

        Save();
    }

    public bool TryGetSteamId(string user, out int steamId)
    {
        steamId = -1;
        if(_users.ContainsKey(user))
        {
            steamId = _users[user].SteamId;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ReadConfig()
    {
        if(File.Exists(FILE) == false)
        {
            _users = new();
            Save();
        }

        _users = JsonSerializer.Deserialize<Dictionary<string, UserInfo>>(File.ReadAllText(FILE));
    }

    private void Save() => File.WriteAllText(FILE, JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true }));
}
