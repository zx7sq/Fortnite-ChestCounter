namespace ChestCounter;

/// <summary>
/// Chests opened before the counter was used, estimated from the number of matches Fortnite shows in the Career tab:
/// (matches played − matches already counted) × the player's own average per match.
/// Example: 300 matches this season, 140 counted at 40 chests on average → (300 − 140) × 40 = 6,400 chests added.
/// </summary>
public static class ChestEstimate
{
    /// <summary>Below this, the average is not reliable enough to be multiplied.</summary>
    public const int MinCountedMatches = 10;

    public sealed record Result(int CareerMatches, int CountedMatches, int MissingMatches, double Average, int Chests);

    /// <summary>The estimate, or null when there are too few counted matches or no match is missing.</summary>
    public static Result? FromCareer(TrackerData view, int careerMatches)
    {
        var counted = view.Matches.Count;
        if (counted < MinCountedMatches || careerMatches <= counted)
        {
            return null;
        }
        var average = (double)view.CountedChests / counted;
        var missing = careerMatches - counted;
        var estimated = (int)Math.Clamp(Math.Round(missing * average, MidpointRounding.AwayFromZero), 0, 10_000_000);
        return new Result(careerMatches, counted, missing, average, estimated);
    }

    /// <summary>"2 500", "2,500", "2.500" or "-40": thousands separators are dropped.</summary>
    public static int? ParseNumber(string text)
    {
        var trimmed = text.TrimStart();
        var negative = trimmed.StartsWith('-');
        var digits = new string(trimmed.Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 0)
        {
            return null;
        }
        return int.TryParse(negative ? "-" + digits : digits, System.Globalization.NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var n) ? n : null;
    }
}
