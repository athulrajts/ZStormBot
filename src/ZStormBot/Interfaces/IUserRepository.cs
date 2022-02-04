namespace ZStormBot.Interfaces;

public interface IUserRepository
{
    void SetSteamId(string user, int steamId);
    bool TryGetSteamId(string user, out int steamId);
}

public class UserInfo
{
    public int SteamId { get; set; }
}