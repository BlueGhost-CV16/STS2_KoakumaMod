using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Cards;

public abstract class KoakumaRareCard : KoakumaCard
{
    protected KoakumaRareCard(int cost, CardType type, TargetType target = TargetType.Self)
        : base(cost, type, CardRarity.Rare, target, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");
}
