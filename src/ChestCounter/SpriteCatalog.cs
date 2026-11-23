using System.Text.RegularExpressions;

namespace ChestCounter;

public sealed record SpriteFamily(string Id, string Name, string Rarity, string Ability, string[] Codes, string[] Variants);

/// <summary>
/// Catalog of Chapter 7: Treasure Hunters Sprites and their variants (Base, Fortune, Cursed, Gold),
/// mapping Fortnite's internal ESD_*Sprite_Variant_* asset names to friendly display names.
/// </summary>
public static class SpriteCatalog
{
    public static readonly string[] AllVariants = { "base", "fortune", "cursed", "gold" };
    public static readonly string[] BaseOnly = { "base" };

    public static readonly SpriteFamily[] Families =
    {
        new("minimiku", "Mini Miku", "Mythic", "Friendship Bubble when emoting with others", new[] { "DaffodilChurn", "MiniMiku", "Miku" }, AllVariants),
        new("zee", "Zee", "Mythic", "Regenerating Zero Point Dash charges", new[] { "Zee", "ZeroPoint" }, BaseOnly),
        new("liluzivert", "Lil' Uzi Vert Jr.", "Legendary", "Shield restoration triggers Chug Splash", new[] { "RookGecko", "LilUzi", "Uzi" }, AllVariants),
        new("lilflacko", "Lil' Flacko", "Legendary", "Landing creates a knockback shockwave", new[] { "PlaymakerBrewery", "LilFlacko", "Flacko", "SpillStrike" }, AllVariants),
        new("siroscar", "Sir Oscar the Second", "Legendary", "Boosts pickaxe damage & speed on shield break", new[] { "WhiteTiger", "SirOscar", "Oscar" }, AllVariants),
        new("squiplash", "Squiplash", "Epic", "Deploys healing smoke when health is low", new[] { "PirateCalamari", "Squiplash" }, AllVariants),
        new("chester", "Chester", "Epic", "Extra chest rewards & negates fall damage", new[] { "TreasureChest", "Chester" }, AllVariants),
        new("sweetdreams", "Sweet Dreams", "Epic", "Grants random loot on level-up, Mythic at max", new[] { "SweetDreams", "SweetDream" }, BaseOnly),
        new("kelper", "Kelper", "Epic", "Gradually recovers health while underwater", new[] { "UnbotheredFish", "Kelper" }, AllVariants),
        new("transpondersnail", "Transponder Snail", "Epic", "Grants & shares bonus resources with teammates", new[] { "FeedJet", "TransponderSnail", "Snail" }, BaseOnly),
        new("halfpeeld", "Halfpeel'd", "Rare", "Marks eliminations within detection area", new[] { "PeelyBones", "PeelyBone", "Halfpeeld" }, AllVariants),
        new("repete", "Re-Pete", "Rare", "Faster reload speed & chance to refund ammo", new[] { "Parrot", "RePete" }, AllVariants),
        new("barryofish", "Barry O'Fish", "Rare", "Lower swim dash stamina & slap effect at max", new[] { "BarrelOfFish", "BarryOFish" }, AllVariants),
        new("exmarks", "Exmarks", "Rare", "Detects nearby treasure maps & wanted posters", new[] { "TreasureMap", "Exmarks" }, AllVariants),
        new("plundarrgh", "Plundarrgh", "Rare", "Pickaxe & movement speed after opening chests", new[] { "CoinPile", "Plundarrgh", "Plunderg" }, AllVariants),
        new("wayfinder", "Wayfinder", "Rare", "Detects Sprites & boosts buried chest loot", new[] { "Compass", "Wayfinder" }, AllVariants),
        new("lilsquibbly", "Lil' Squibbly", "Rare", "Dives into fishing spots for fish & rewards", new[] { "Squibbly", "LilSquibbly" }, AllVariants),
    };

    private static readonly Dictionary<string, SpriteFamily> ByCode = BuildCodeMap();
    private static readonly Dictionary<string, SpriteFamily> ById = Families.ToDictionary(f => f.Id, StringComparer.OrdinalIgnoreCase);

    // Matches: ESD_TreasureChestSprite_Variant_LiquidGold or .../ESD_PeelyBoneSprite_Variant_A
    private static readonly Regex EsdRegex = new(@"ESD_([A-Za-z0-9]+?)Sprite_Variant_([A-Za-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static Dictionary<string, SpriteFamily> BuildCodeMap()
    {
        var map = new Dictionary<string, SpriteFamily>(StringComparer.OrdinalIgnoreCase);
        foreach (var f in Families)
        {
            map[f.Id] = f;
            foreach (var c in f.Codes) map[c] = f;
        }
        return map;
    }

    public static SpriteFamily? FindFamily(string idOrCode) =>
        ById.TryGetValue(idOrCode, out var f) ? f : ByCode.TryGetValue(idOrCode, out var f2) ? f2 : null;

    public static string VariantLabel(string variant) => variant switch
    {
        "fortune" => "Fortune",
        "cursed" => "Cursed",
        "gold" => "Gold",
        _ => "Base",
    };

    public static string MapVariant(string raw)
    {
        var v = raw.Trim().ToLowerInvariant();
        if (v.Contains("gold")) return "gold";
        if (v.Contains("cursed")) return "cursed";
        if (v.Contains("jewel") || v.Contains("fortune")) return "fortune";
        return "base";
    }

    /// <summary>Parses a Fortnite sprite asset path (e.g. ESD_WhiteTigerSprite_Variant_LiquidGold) into (familyId, variant, displayName).</summary>
    public static (string FamilyId, string Variant, string DisplayName)? ParseDefinition(string? pathOrName)
    {
        if (string.IsNullOrWhiteSpace(pathOrName)) return null;
        var m = EsdRegex.Match(pathOrName);
        if (!m.Success) return null;

        var code = m.Groups[1].Value;
        var variant = MapVariant(m.Groups[2].Value);

        if (ByCode.TryGetValue(code, out var fam))
        {
            var label = variant == "base" ? fam.Name : $"{VariantLabel(variant)} {fam.Name}";
            return (fam.Id, variant, label);
        }

        var nice = Regex.Replace(code, "([a-z])([A-Z])", "$1 $2");
        var id = code.ToLowerInvariant();
        var fallbackLabel = variant == "base" ? nice : $"{VariantLabel(variant)} {nice}";
        return (id, variant, fallbackLabel);
    }
}
