using Unreal.Core.Attributes;
using Unreal.Core.Contracts;
using Unreal.Core.Models.Enums;

namespace FortniteReplayReader.Models.NetFieldExports.Items;

/// <summary>
/// ChestCounter patch: equipped Sprite backpack actor attached to a player's pawn.
/// Replicates which Sprite + variant (ESD_*Sprite_Variant_*) the player is carrying and whether it reached max level.
/// </summary>
[NetFieldExportGroup("/SpriteExtractionGameplay/Gameplay/Backpack/BP_ExtractionBackpack_Sprite.BP_ExtractionBackpack_Sprite_C", minimalParseMode: ParseMode.Minimal)]
public class ExtractionBackpackSprite : INetFieldExportGroup
{
    [NetFieldExport("Owner", RepLayoutCmdType.PropertyObject)] public uint? Owner { get; set; }
    [NetFieldExport("Instigator", RepLayoutCmdType.PropertyObject)] public uint? Instigator { get; set; }
    [NetFieldExport("OwningPawn", RepLayoutCmdType.PropertyObject)] public uint? OwningPawn { get; set; }
    [NetFieldExport("ExtractableItemDefinition", RepLayoutCmdType.PropertyObject)] public uint? ExtractableItemDefinition { get; set; }
    [NetFieldExport("SpriteHidden", RepLayoutCmdType.PropertyBool)] public bool? SpriteHidden { get; set; }
    [NetFieldExport("bIsMaxLevel", RepLayoutCmdType.PropertyBool)] public bool? bIsMaxLevel { get; set; }
}
