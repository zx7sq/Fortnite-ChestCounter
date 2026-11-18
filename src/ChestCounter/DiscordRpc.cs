using System.Buffers.Binary;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// Lightweight, dependency-free Discord Rich Presence client over Discord's local named pipe (discord-ipc-0..9).
/// Never touches Fortnite or any game process.
/// </summary>
public static class DiscordRpc
{
    private static readonly object Gate = new();
    private static NamedPipeClientStream? _pipe;
    private static string _connectedAppId = "";
    private static string _lastPayloadKey = "";
    private static long _lastAttemptMs;
    private static readonly long SessionStartUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static void Update(bool enabled, string? appId, string details, string state)
    {
        var id = appId?.Trim() ?? "";
        if (!enabled || id.Length < 17 || !id.All(char.IsDigit))
        {
            Clear();
            return;
        }

        var key = $"{id}|{details}|{state}";
        lock (Gate)
        {
            if (_pipe is { IsConnected: true } && _connectedAppId == id && _lastPayloadKey == key)
            {
                return;
            }
            var now = Environment.TickCount64;
            if ((_pipe == null || !_pipe.IsConnected) && now - _lastAttemptMs < 15_000)
            {
                return;
            }
            _lastAttemptMs = now;
        }

        _ = Task.Run(() => SendLocked(id, details, state, key));
    }

    public static void Clear()
    {
        lock (Gate)
        {
            _lastPayloadKey = "";
            _connectedAppId = "";
            try { _pipe?.Dispose(); } catch { }
            _pipe = null;
        }
    }

    private static void SendLocked(string appId, string details, string state, string key)
    {
        lock (Gate)
        {
            try
            {
                if (_pipe is not { IsConnected: true } || _connectedAppId != appId)
                {
                    try { _pipe?.Dispose(); } catch { }
                    _pipe = Connect(appId);
                    if (_pipe == null)
                    {
                        _connectedAppId = "";
                        return;
                    }
                    _connectedAppId = appId;
                }

                var frame = JsonSerializer.Serialize(new
                {
                    cmd = "SET_ACTIVITY",
                    args = new
                    {
                        pid = Environment.ProcessId,
                        activity = new
                        {
                            details,
                            state,
                            timestamps = new { start = SessionStartUnix },
                        },
                    },
                    nonce = Guid.NewGuid().ToString("N"),
                });
                WritePacket(_pipe, 1, frame);
                DrainNonBlocking(_pipe);
                _lastPayloadKey = key;
            }
            catch
            {
                try { _pipe?.Dispose(); } catch { }
                _pipe = null;
                _connectedAppId = "";
                _lastPayloadKey = "";
            }
        }
    }

    private static NamedPipeClientStream? Connect(string appId)
    {
        for (var i = 0; i < 10; i++)
        {
            NamedPipeClientStream? pipe = null;
            try
            {
                pipe = new NamedPipeClientStream(".", $"discord-ipc-{i}", PipeDirection.InOut, PipeOptions.Asynchronous);
                pipe.Connect(120);
                WritePacket(pipe, 0, JsonSerializer.Serialize(new { v = 1, client_id = appId }));
                DrainNonBlocking(pipe);
                return pipe;
            }
            catch
            {
                try { pipe?.Dispose(); } catch { }
            }
        }
        return null;
    }

    private static void WritePacket(NamedPipeClientStream pipe, int opcode, string json)
    {
        var body = Encoding.UTF8.GetBytes(json);
        var buf = new byte[8 + body.Length];
        BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan(0, 4), opcode);
        BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan(4, 4), body.Length);
        Buffer.BlockCopy(body, 0, buf, 8, body.Length);
        pipe.Write(buf, 0, buf.Length);
        pipe.Flush();
    }

    private static void DrainNonBlocking(NamedPipeClientStream pipe)
    {
        // Read handshake/ack if ready within a short window without ever blocking the caller.
        try
        {
            var header = new byte[8];
            var readTask = pipe.ReadAsync(header, 0, 8);
            if (!readTask.Wait(200) || readTask.Result < 8)
            {
                return;
            }
            var len = BinaryPrimitives.ReadInt32LittleEndian(header.AsSpan(4, 4));
            if (len is > 0 and < 65536)
            {
                var payload = new byte[len];
                pipe.ReadAsync(payload, 0, len).Wait(200);
            }
        }
        catch { }
    }
}
