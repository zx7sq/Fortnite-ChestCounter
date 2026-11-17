namespace Unreal.Core;

/// <summary>
/// ChestCounter patch: diagnostic probe. When <see cref="Sink"/> is set, the raw bits of every property / RPC / FastArray
/// that the parser does NOT decode are handed to it, for groups whose path matches <see cref="Filter"/>.
/// Used to discover where a piece of information lives in the replay. Null (disabled) by default.
/// </summary>
public static class RawProbe
{
    public delegate void SinkHandler(uint channelIndex, string group, string field, byte[] data, int bits);

    [ThreadStatic]
    private static Func<string, bool>? _filter;

    [ThreadStatic]
    private static SinkHandler? _sink;

    public static Func<string, bool>? Filter
    {
        get => _filter;
        set => _filter = value;
    }

    public static SinkHandler? Sink
    {
        get => _sink;
        set => _sink = value;
    }

    internal static bool Matches(string path) => _sink != null && _filter?.Invoke(path) == true;
}
