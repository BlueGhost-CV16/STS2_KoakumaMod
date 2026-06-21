using Godot;
using BG_Koakuma.Characters;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace BG_Koakuma.Cards;

public sealed class BG_KoakumaMagicBookCardPool : TypeListCardPoolModel
{
    private static readonly Material? PoolFrameTintMaterial =
        MaterialUtils.CreateHsvShaderMaterial(0.975f, 0.850f, 1.35f);

    public override string Title => "BG_Koakuma_MagicBooks";
    public override string EnergyColorName => "BG_Koakuma";

    public override string? BigEnergyIconPath => $"{Entry.ResPath}/images/characters/energy_big.png";
    public override string? TextEnergyIconPath => $"{Entry.ResPath}/images/characters/energy_text.png";

    public override Color DeckEntryCardColor => BG_KoakumaCharacter.ThemeColor;
    public override Color EnergyOutlineColor => new(0.08f, 0.18f, 0.24f);
    public override Material? PoolFrameMaterial => PoolFrameTintMaterial;
    public override bool IsColorless => false;
}
