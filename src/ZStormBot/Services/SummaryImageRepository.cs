using CoreHtmlToImage;
using Microsoft.Extensions.Configuration;
using OpenDotaApi.Api.Matches.Model;
using OpenDotaApi.Api.Players.Model.RecentMatches;
using System.Text;
using System.Text.Json;
using ZStormBot.Interfaces;
using PlayerMatch = OpenDotaApi.Api.Players.Model.Matches.Matches;

namespace ZStormBot.Services;

public class SummaryImageRepository : ISummaryImageRepository
{
    private readonly string _apiKey;
    private readonly IDotaConstants _constants;

    public SummaryImageRepository(IDotaConstants heroRepository, IConfiguration configuration)
    {
        _constants = heroRepository;
        _apiKey = configuration.GetValue<string>(AppSettings.ImageHostingToken);
    }
    
    public static string GetFormated(int? value, double amount = 1000.0) => (value == 0 || !value.HasValue) ? "-" : $"{value / amount:N1}k";
    public static string GetFormated(long? value, double amount = 1000.0) => (value == 0 || !value.HasValue) ? "-" : $"{value / amount:N1}k";

    public async Task<string> RecentGames(IEnumerable<RecentMatches> matches)
    {
        var html = $@"
<html>
    <head>
    <style>
        table {{
          border-collapse: collapse;
          width: 2560px;
        }}

        th, td {{
          text-align: left;
          padding: 8px;
          font-size : 70px;
        }}
        .win{{color:green;}}
        .loss{{color:red;}}
        tr:nth-child(even) {{background-color: #f2f2f2;}}
        </style>
    </head>
    <body>
        <table>
            <tr>
                <th>Hero</th>
                <th>Result</th>
                <th>KDA</th>
                <th>GPM</th>
                <th>XPM</th>
                <th>TD</th>
                <th>HD</th>
                <th>Duration<th>
            </tr>
            {GetMatchStats(matches)}
        </table>
    </body>
</html>";

        return await GetImageUrl(html);
    }
    
    public async Task<string> LastGame(RecentMatches match)
    {
        var html = $@"
<html>
    <head>
    <style>
        table {{
          border-collapse: collapse;
          width: 100%;
        }}

        th, td {{
          text-align: left;
          padding: 8px;
          font-size : 50px;
        }}
        .win{{color:green;}}
        .loss{{color:red;}}
        tr:nth-child(even) {{background-color: #f2f2f2;}}
        </style>
    </head>
    <body>
        <table>
            <tr>
                <th>Hero</th>
                <th>Result</th>
                <th>KDA</th>
                <th>GPM</th>
                <th>XPM</th>
                <th>TD</th>
                <th>HD</th>
                <th>Duration<th>
            </tr>
            {GetMatchStats(match)}
        </table>
    </body>
</html>";

        return await GetImageUrl(html);
    }

    public async Task<string> LastGameFull(Match match)
    {
        var radiantPlayers = match.Players.Where(x => x.PlayerSlot <= 127).ToList();
        var direPlayers = match.Players.Where(x => x.PlayerSlot > 127).ToList();
        var html = $@"
<html>
    <head>
    <style>
        table {{
          border-collapse: collapse;
          width: 2560px;
        }}

        th, td {{
          text-align: center;
          padding: 8px;
          font-size : 70px;
        }}
        .win{{color:green;}}
        .loss{{color:red;}}
        .header{{font-size:80px;}}
        .left{{text-align: left;}}
        tr:nth-child(even) {{background-color: #f2f2f2;}}
        </style>
    </head>
    <body>
        <h1 class='win header'>Radiant</h1>
        <table>
            <tr>
                <th>Hero</th>
                <th width='20%'>Player</th>
                <th>KDA</th>
                <th>LH/DN</th>
                <th>GPM/XPM</th>
                <th>DMG/BLD</th>
                <th>Heal</th>
            </tr>
            {GetMatchTable(radiantPlayers)}
        </table>
        <br/>
        <h1 class='loss header'>Dire</h1>
        <table>
            <tr>
                <th>Hero</th>
                <th width='20%'>Player</th>
                <th>KDA</th>
                <th>LH/DN</th>
                <th>GPM/XPM</th>
                <th>DMG/BLD</th>
                <th>Heal</th>
            </tr>
            {GetMatchTable(direPlayers)}
        </table>
    </body>
</html>
";

        return await GetImageUrl(html);
    }

    public async Task<string> RecentGamesAs(IEnumerable<MatchPlayer> matches)
    {
        var html = $@"
<html>
    <head>
    <style>
        table {{
          border-collapse: collapse;
          width: 2560px;
        }}

        th, td {{
          text-align: left;
          padding: 8px;
          font-size : 70px;
        }}
        .win{{color:green;}}
        .loss{{color:red;}}
        tr:nth-child(even) {{background-color: #f2f2f2;}}
        </style>
    </head>
    <body>
        <table>
            <tr>
                <th>Hero</th>
                <th>Result</th>
                <th width='20%'>Player</th>
                <th>KDA</th>
                <th>LH/DN</th>
                <th>GPM/XPM</th>
                <th>DMG/BLD</th>
                <th>Heal</th>
            </tr>
            {GetRecentGames(matches)}
        </table>
    </body>
</html>";

        return await GetImageUrl(html);
    }

    public async Task<string> RecentGamesWithOrAgainst(IEnumerable<MatchPlayer> matches)
    {
        var html = $@"
<html>
    <head>
    <style>
        table {{
          border-collapse: collapse;
          width: 2560px;
        }}

        th, td {{
          text-align: left;
          padding: 8px;
          font-size : 70px;
        }}
        .win{{color:green;}}
        .loss{{color:red;}}
        tr:nth-child(even) {{background-color: #f2f2f2;}}
        </style>
    </head>
    <body>
        <table>
            <tr>
                <th>Hero</th>
                <th>Result</th>
                <th width='20%'>Player</th>
                <th>KDA</th>
                <th>LH/DN</th>
                <th>GPM/XPM</th>
                <th>DMG/BLD</th>
                <th>Heal</th>
            </tr>
            {GetRecentGames(matches)}
        </table>
    </body>
</html>";

        return await GetImageUrl(html);
    }


    public async Task<string> GetImageUrl(string html)
    {
        var converter = new HtmlConverter();
        var bytes = converter.FromHtmlString(html);
        var image = Convert.ToBase64String(bytes);
        string endPoint = @$"https://api.imgbb.com/1/upload?expiration=600&key={_apiKey}";
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(new HttpMethod("POST"), endPoint);
        var multipartContent = new MultipartFormDataContent
        {
            { new StringContent(image), "image" }
        };
        request.Content = multipartContent;
        var response = await httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Rootobject>(json);
        return result.data.display_url;
    }

    private string GetMatchTable(IEnumerable<MatchPlayer> players)
    {
        var sb = new StringBuilder();
        foreach (var player in players)
        {
            sb.AppendLine("<tr>");

            var kda = $"{player.Kills}/{player.Deaths}/{player.Assists}";
            sb.AppendLine($"<td><img src='{_constants.GetHeroThumbnail(player.HeroId)}' Height='100' Width='178'/></td>");
            sb.AppendLine($"<td width='20%' class='left'>{player.Personaname ?? "Annonymous"}</td>");
            sb.AppendLine($"<td>{kda}</td>");
            sb.AppendLine($"<td>{player.LastHits}/{player.Denies}</td>");
            sb.AppendLine($"<td>{player.GoldPerMin}/{player.XpPerMin}</td>");
            sb.AppendLine($"<td>{GetFormated(player.HeroDamage)}/{GetFormated(player.TowerDamage)}</td>");
            sb.AppendLine($"<td>{GetFormated(player.HeroHealing)}</td>");
            sb.AppendLine("</tr>");
        }
        return sb.ToString();
    }

    private string GetRecentGames(IEnumerable<MatchPlayer> players)
    {
        var sb = new StringBuilder();
        foreach (var player in players)
        {
            var isPlayerRadiant = player.PlayerSlot <= 127;
            var playerWon = isPlayerRadiant ? player.RadiantWin.Value : !player.RadiantWin.Value;
            sb.AppendLine("<tr>");

            var kda = $"{player.Kills}/{player.Deaths}/{player.Assists}";
            sb.AppendLine($"<td><img src='{_constants.GetHeroThumbnail(player.HeroId)}' Height='100' Width='178'/></td>");
            sb.AppendLine($"<td class='{(playerWon ? "win" : "loss")}'>{(playerWon ? "Win" : "Loss")}</td>");
            sb.AppendLine($"<td width='20%' class='left'>{player.Personaname ?? "Annonymous"}</td>");
            sb.AppendLine($"<td>{kda}</td>");
            sb.AppendLine($"<td>{player.LastHits}/{player.Denies}</td>");
            sb.AppendLine($"<td>{player.GoldPerMin}/{player.XpPerMin}</td>");
            sb.AppendLine($"<td>{GetFormated(player.HeroDamage)}/{GetFormated(player.TowerDamage)}</td>");
            sb.AppendLine($"<td>{GetFormated(player.HeroHealing)}</td>");
            sb.AppendLine("</tr>");
        }
        return sb.ToString();
    }

    private string GetMatchStats(IEnumerable<RecentMatches> matches)
    {
        var sb = new StringBuilder();

        foreach (var match in matches)
        {
            sb.AppendLine(GetMatchStats(match));
        }

        return sb.ToString();
    }

    private string GetMatchStats(IEnumerable<PlayerMatch> matches)
    {
        var sb = new StringBuilder();

        foreach (var match in matches)
        {
            sb.AppendLine(GetMatchStats(match));
        }

        return sb.ToString();
    }


    private string GetMatchStats(RecentMatches match)
    {
        var isPlayerRadiant = match.PlayerSlot <= 127;
        var playerWon = isPlayerRadiant ? match.RadiantWin.Value : !match.RadiantWin.Value;

        return @$"
            <tr>
                <td><img src='{_constants.GetHeroThumbnail(match.HeroId)}' Height='100' Width='178'/></td>
                <td class='{(playerWon ? "win" : "loss")}'>{(playerWon ? "Win" : "Loss")}</td>
                <td>{match.Kills}/{match.Deaths}/{match.Assists}</td>
                <td>{match.GoldPerMin}</td>
                <td>{match.XpPerMin}</td>
                <td>{GetFormated(match.TowerDamage.Value)}</td>
                <td>{GetFormated(match.HeroDamage.Value)}</td>
                <td>{TimeSpan.FromSeconds(match.Duration.Value)}</td>
            </tr>";
    }

    private string GetMatchStatsWithoutHero(MatchPlayer match)
    {
        var isPlayerRadiant = match.PlayerSlot <= 127;
        var playerWon = isPlayerRadiant ? match.RadiantWin.Value : !match.RadiantWin.Value;

        return @$"
            <tr>
                <td>{match.MatchId}</td>
                <td class='{(playerWon ? "win" : "loss")}'>{(playerWon ? "Win" : "Loss")}</td>
                <td>{match.Kills}/{match.Deaths}/{match.Assists}</td>
                <td>{TimeSpan.FromSeconds(match.Duration.Value)}</td>
            </tr>";
    }

    private string GetMatchStats(PlayerMatch match)
    {
        var isPlayerRadiant = match.PlayerSlot <= 127;
        var playerWon = isPlayerRadiant ? match.RadiantWin.Value : !match.RadiantWin.Value;

        return @$"
            <tr>
                <td><img src='{_constants.GetHeroThumbnail(match.HeroId)}' Height='100' Width='178'/></td>
                <td>{match.MatchId}</td>
                <td class='{(playerWon ? "win" : "loss")}'>{(playerWon ? "Win" : "Loss")}</td>
                <td>{match.Kills}/{match.Deaths}/{match.Assists}</td>
                <td>{TimeSpan.FromSeconds(match.Duration.Value)}</td>
            </tr>";
    }
}


public class Rootobject
{
    public Data data { get; set; }
    public bool success { get; set; }
    public int status { get; set; }
}

public class Data
{
    public string id { get; set; }
    public string title { get; set; }
    public string url_viewer { get; set; }
    public string url { get; set; }
    public string display_url { get; set; }
    public int size { get; set; }
    public string time { get; set; }
    public string expiration { get; set; }
    public Image image { get; set; }
    public Thumb thumb { get; set; }
    public Medium medium { get; set; }
    public string delete_url { get; set; }
}

public class Image
{
    public string filename { get; set; }
    public string name { get; set; }
    public string mime { get; set; }
    public string extension { get; set; }
    public string url { get; set; }
}

public class Thumb
{
    public string filename { get; set; }
    public string name { get; set; }
    public string mime { get; set; }
    public string extension { get; set; }
    public string url { get; set; }
}

public class Medium
{
    public string filename { get; set; }
    public string name { get; set; }
    public string mime { get; set; }
    public string extension { get; set; }
    public string url { get; set; }
}