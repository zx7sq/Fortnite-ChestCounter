using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// Fetches live Fortnite player stats and outfit artwork from fortnite-api.com for the Stats tab.
/// </summary>
public static class FortniteStatsClient
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private static readonly ConcurrentDictionary<string, string> SkinCache = new(StringComparer.OrdinalIgnoreCase);

    static FortniteStatsClient()
    {
        Http.DefaultRequestHeaders.UserAgent.ParseAdd($"FortniteChestCounter/{AppInfo.VersionText}");
    }

    public static (int Status, string Json) FetchStats(string name, string accountType, string timeWindow, string apiKey)
    {
        name = (name ?? "").Trim();
        if (name.Length == 0)
        {
            return (400, JsonSerializer.Serialize(new { error = "Please enter a Fortnite username." }));
        }

        apiKey = (apiKey ?? "").Trim();
        if (apiKey.Length == 0)
        {
            return (401, JsonSerializer.Serialize(new
            {
                code = "NO_API_KEY",
                error = "No Fortnite API key configured. Click API KEY to add your free key from dash.fortnite-api.com."
            }));
        }

        var acct = accountType is "psn" or "xbl" ? accountType : "epic";
        var win = timeWindow == "lifetime" ? "lifetime" : "season";
        var url = $"https://fortnite-api.com/v2/stats/br/v2?name={Uri.EscapeDataString(name)}&accountType={acct}&timeWindow={win}";

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.TryAddWithoutValidation("Authorization", apiKey);
            using var res = Http.Send(req);
            var body = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var status = (int)res.StatusCode;

            if (res.IsSuccessStatusCode)
            {
                return (200, body);
            }

            if (status == 404)
            {
                return (404, JsonSerializer.Serialize(new { error = $"Player \"{name}\" was not found on {acct.ToUpperInvariant()}." }));
            }
            if (status == 403)
            {
                return (403, JsonSerializer.Serialize(new { error = $"Stats for \"{name}\" are private. Enable \"Show on Career Leaderboard\" in Fortnite settings." }));
            }
            if (status == 401)
            {
                return (401, JsonSerializer.Serialize(new { code = "INVALID_API_KEY", error = "Invalid Fortnite API key. Check your key in API KEY settings." }));
            }

            string? apiErr = null;
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var e))
                {
                    apiErr = e.GetString();
                }
            }
            catch { }

            return (status, JsonSerializer.Serialize(new { error = apiErr ?? $"Fortnite API returned status {status}." }));
        }
        catch
        {
            return (502, JsonSerializer.Serialize(new { error = "Failed to reach fortnite-api.com. Please try again." }));
        }
    }

    public static (int Status, string Json) FetchSkin(string skinName)
    {
        var key = (skinName ?? "").Trim();
        if (key.Length == 0) key = "Midas";

        if (SkinCache.TryGetValue(key, out var cached))
        {
            return (200, cached);
        }

        var url = $"https://fortnite-api.com/v2/cosmetics/br/search?name={Uri.EscapeDataString(key)}&type=outfit&matchMethod=contains";
        try
        {
            using var res = Http.GetAsync(url).GetAwaiter().GetResult();
            if (!res.IsSuccessStatusCode)
            {
                return ((int)res.StatusCode, JsonSerializer.Serialize(new { error = "Outfit not found." }));
            }

            var body = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("data", out var item) || item.ValueKind != JsonValueKind.Object)
            {
                return (404, JsonSerializer.Serialize(new { error = "Outfit not found." }));
            }

            string? image = null;
            if (item.TryGetProperty("images", out var imgs) && imgs.ValueKind == JsonValueKind.Object)
            {
                if (imgs.TryGetProperty("featured", out var f) && f.ValueKind == JsonValueKind.String) image = f.GetString();
                else if (imgs.TryGetProperty("icon", out var i) && i.ValueKind == JsonValueKind.String) image = i.GetString();
                else if (imgs.TryGetProperty("smallIcon", out var s) && s.ValueKind == JsonValueKind.String) image = s.GetString();
            }

            var result = JsonSerializer.Serialize(new
            {
                id = item.TryGetProperty("id", out var id) ? id.GetString() : null,
                name = item.TryGetProperty("name", out var n) ? n.GetString() : key,
                rarity = item.TryGetProperty("rarity", out var r) && r.TryGetProperty("displayValue", out var rv) ? rv.GetString() : "Epic",
                image
            });

            if (SkinCache.Count < 100) SkinCache[key] = result;
            return (200, result);
        }
        catch
        {
            return (502, JsonSerializer.Serialize(new { error = "Failed to fetch outfit artwork." }));
        }
    }
}
