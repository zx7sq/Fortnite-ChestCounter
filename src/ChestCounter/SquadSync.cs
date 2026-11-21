using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// Automatically detects squadmates in the same Fortnite match who are also running ChestCounter
/// and exchanges exact chest counts via a zero-setup public relay (ntfy.sh) keyed by a one-way
/// SHA-256 hash of (SessionId, TeamIndex). All network I/O is non-blocking and silent on failure.
/// </summary>
public sealed class SquadSync
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(8) };

    static SquadSync()
    {
        Http.DefaultRequestHeaders.UserAgent.ParseAdd($"FortniteChestCounter/{AppInfo.VersionText}");
    }

    private readonly object _gate = new();
    private string? _liveSessionId;
    private Dictionary<string, SyncedSquadMember> _livePeers = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(string SessionId, string PeerId, SyncedSquadMember Member)> _pendingMerges = new();
    private readonly Dictionary<string, (int Chests, bool Live)> _lastPublished = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DateTime> _lastPolled = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, (string Topic, string SelfId, DateTime UntilUtc)> _recentMatches = new(StringComparer.OrdinalIgnoreCase);
    private int _inFlight;

    /// <summary>Snapshot of detected app-using squadmates in the currently live match.</summary>
    public (string? SessionId, List<SyncedSquadMember> Peers) LiveSnapshot()
    {
        lock (_gate)
        {
            return (_liveSessionId, _livePeers.Values.OrderByDescending(p => p.Chests).ToList());
        }
    }

    /// <summary>
    /// Drains any newly received squadmate counts into <see cref="TrackerData.Matches"/> on the main loop thread.
    /// Returns true if any match record changed (so the caller can save data.json).
    /// </summary>
    public bool ApplyPending(TrackerData data)
    {
        List<(string SessionId, string PeerId, SyncedSquadMember Member)> batch;
        lock (_gate)
        {
            if (_pendingMerges.Count == 0) return false;
            batch = new(_pendingMerges);
            _pendingMerges.Clear();
        }

        var changed = false;
        foreach (var (sessionId, peerId, member) in batch)
        {
            var match = data.Matches.FirstOrDefault(m => m.SessionId == sessionId);
            if (match == null) continue;
            if (!match.SyncedSquad.TryGetValue(peerId, out var existing)
                || member.Chests > existing.Chests
                || (existing.Live && !member.Live)
                || (string.IsNullOrEmpty(existing.Name) && !string.IsNullOrEmpty(member.Name)))
            {
                match.SyncedSquad[peerId] = new SyncedSquadMember
                {
                    Name = !string.IsNullOrEmpty(member.Name) ? member.Name : (existing?.Name ?? "Teammate"),
                    Chests = Math.Max(member.Chests, existing?.Chests ?? 0),
                    Live = member.Live && (existing?.Live ?? true),
                    UpdatedUtc = member.UpdatedUtc,
                };
                changed = true;
            }
        }
        return changed;
    }

    /// <summary>Called periodically from the main loop for the live match (if any) and recently finished squad matches.</summary>
    public void Tick(bool enabled, MatchRecord? liveMatch, bool isLive)
    {
        if (!enabled)
        {
            lock (_gate)
            {
                _liveSessionId = null;
                _livePeers.Clear();
            }
            return;
        }

        var now = DateTime.UtcNow;

        if (isLive && liveMatch != null && TryBuildRoom(liveMatch, out var topic, out var selfId, out var selfName))
        {
            lock (_gate)
            {
                if (_liveSessionId != liveMatch.SessionId)
                {
                    _liveSessionId = liveMatch.SessionId;
                    _livePeers = new(StringComparer.OrdinalIgnoreCase);
                }
            }
            ScheduleSync(liveMatch.SessionId, topic, selfId, selfName, liveMatch.ChestsOpened, isLive: true, now);
        }
        else if (!isLive)
        {
            lock (_gate)
            {
                _liveSessionId = null;
                _livePeers.Clear();
            }
        }

        // Continue polling recently finished squad matches for 90s to catch teammates who finish slightly later.
        List<(string SessionId, string Topic, string SelfId)> toPoll = new();
        lock (_gate)
        {
            foreach (var kv in _recentMatches.ToList())
            {
                if (now > kv.Value.UntilUtc)
                {
                    _recentMatches.Remove(kv.Key);
                    continue;
                }
                if (!_lastPolled.TryGetValue(kv.Value.Topic, out var last) || (now - last).TotalSeconds >= 25)
                {
                    _lastPolled[kv.Value.Topic] = now;
                    toPoll.Add((kv.Key, kv.Value.Topic, kv.Value.SelfId));
                }
            }
        }

        foreach (var item in toPoll)
        {
            PollAsync(item.SessionId, item.Topic, item.SelfId, isLiveMatch: false);
        }
    }

    /// <summary>Called when a match finishes and has been added to <see cref="TrackerData.Matches"/>.</summary>
    public void OnMatchFinished(bool enabled, MatchRecord match)
    {
        if (!enabled || !TryBuildRoom(match, out var topic, out var selfId, out var selfName))
        {
            return;
        }

        // Carry over any live peers already discovered during this match so they appear immediately.
        lock (_gate)
        {
            if (_liveSessionId == match.SessionId && _livePeers.Count > 0)
            {
                foreach (var (peerId, member) in _livePeers)
                {
                    _pendingMerges.Add((match.SessionId, peerId, member));
                }
            }
            _recentMatches[match.SessionId] = (topic, selfId, DateTime.UtcNow.AddSeconds(95));
        }

        ScheduleSync(match.SessionId, topic, selfId, selfName, match.ChestsOpened, isLive: false, DateTime.UtcNow);
    }

    private void ScheduleSync(string sessionId, string topic, string selfId, string selfName, int chests, bool isLive, DateTime now)
    {
        var shouldPublish = false;
        var shouldPoll = false;
        lock (_gate)
        {
            if (!_lastPublished.TryGetValue(topic, out var prev) || prev.Chests != chests || prev.Live != isLive)
            {
                _lastPublished[topic] = (chests, isLive);
                shouldPublish = true;
            }
            if (!_lastPolled.TryGetValue(topic, out var last) || (now - last).TotalSeconds >= 25 || !isLive)
            {
                _lastPolled[topic] = now;
                shouldPoll = true;
            }
        }

        if (shouldPublish)
        {
            PublishAsync(topic, selfId, selfName, chests, isLive);
        }
        if (shouldPoll)
        {
            PollAsync(sessionId, topic, selfId, isLiveMatch: isLive);
        }
    }

    private void PublishAsync(string topic, string selfId, string selfName, int chests, bool isLive)
    {
        if (Interlocked.Increment(ref _inFlight) > 4)
        {
            Interlocked.Decrement(ref _inFlight);
            return;
        }
        _ = Task.Run(async () =>
        {
            try
            {
                var payload = JsonSerializer.Serialize(new { id = selfId, n = selfName, c = chests, l = isLive });
                using var content = new StringContent(payload, Encoding.UTF8, "text/plain");
                using var res = await Http.PostAsync($"https://ntfy.sh/{topic}", content).ConfigureAwait(false);
            }
            catch
            {
                // Offline or transient relay error: ignore silently.
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        });
    }

    private void PollAsync(string sessionId, string topic, string selfId, bool isLiveMatch)
    {
        if (Interlocked.Increment(ref _inFlight) > 4)
        {
            Interlocked.Decrement(ref _inFlight);
            return;
        }
        _ = Task.Run(async () =>
        {
            try
            {
                using var res = await Http.GetAsync($"https://ntfy.sh/{topic}/json?poll=1&since=2h").ConfigureAwait(false);
                if (!res.IsSuccessStatusCode) return;
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(body)) return;

                var discovered = new Dictionary<string, SyncedSquadMember>(StringComparer.OrdinalIgnoreCase);
                foreach (var line in body.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        using var outer = JsonDocument.Parse(line);
                        if (!outer.RootElement.TryGetProperty("event", out var ev) || ev.GetString() != "message") continue;
                        if (!outer.RootElement.TryGetProperty("message", out var msgEl)) continue;
                        var msgStr = msgEl.GetString();
                        if (string.IsNullOrWhiteSpace(msgStr)) continue;

                        using var inner = JsonDocument.Parse(msgStr);
                        var r = inner.RootElement;
                        var peerId = r.TryGetProperty("id", out var idEl) ? idEl.GetString()?.Trim() : null;
                        if (string.IsNullOrEmpty(peerId) || string.Equals(peerId, selfId, StringComparison.OrdinalIgnoreCase)) continue;
                        var peerName = r.TryGetProperty("n", out var nEl) ? nEl.GetString()?.Trim() ?? "Teammate" : "Teammate";
                        var peerChests = r.TryGetProperty("c", out var cEl) && cEl.TryGetInt32(out var ci) ? Math.Clamp(ci, 0, 500) : 0;
                        var peerLive = r.TryGetProperty("l", out var lEl) && lEl.ValueKind == JsonValueKind.True;

                        if (!discovered.TryGetValue(peerId, out var prev) || peerChests > prev.Chests || (prev.Live && !peerLive))
                        {
                            discovered[peerId] = new SyncedSquadMember
                            {
                                Name = peerName,
                                Chests = Math.Max(peerChests, prev?.Chests ?? 0),
                                Live = peerLive && (prev?.Live ?? true),
                                UpdatedUtc = DateTime.UtcNow,
                            };
                        }
                    }
                    catch
                    {
                        // Ignore malformed lines
                    }
                }

                if (discovered.Count == 0) return;

                lock (_gate)
                {
                    if (isLiveMatch && _liveSessionId == sessionId)
                    {
                        foreach (var (peerId, member) in discovered)
                        {
                            if (!_livePeers.TryGetValue(peerId, out var cur) || member.Chests > cur.Chests || (cur.Live && !member.Live))
                            {
                                _livePeers[peerId] = member;
                            }
                        }
                    }
                    foreach (var (peerId, member) in discovered)
                    {
                        _pendingMerges.Add((sessionId, peerId, member));
                    }
                }
            }
            catch
            {
                // Offline or transient relay error: ignore silently.
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        });
    }

    private static bool TryBuildRoom(MatchRecord match, out string topic, out string selfId, out string selfName)
    {
        topic = "";
        selfId = "";
        selfName = "";

        if (string.IsNullOrWhiteSpace(match.SessionId)
            || match.SessionId.EndsWith(".replay", StringComparison.OrdinalIgnoreCase)
            || match.TeamIndex == null
            || !match.TeamChests.Any(t => !t.IsBot))
        {
            return false;
        }

        selfId = !string.IsNullOrWhiteSpace(match.EpicId) ? match.EpicId.Trim() : (match.PlayerName ?? "").Trim();
        if (selfId.Length == 0) return false;
        selfName = !string.IsNullOrWhiteSpace(match.PlayerName) ? match.PlayerName.Trim() : "Player";

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"v1:{match.SessionId.Trim()}:{match.TeamIndex.Value}"));
        topic = "ccsq-" + Convert.ToHexString(hash).ToLowerInvariant()[..24];
        return true;
    }
}
