using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaAncientCard : KoakumaCard
{
    protected KoakumaAncientCard(int cost, CardType type, TargetType target = TargetType.Self)
        : base(cost, type, CardRarity.Ancient, target, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");
}
