using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BG_Koakuma.Cards;
using BG_Koakuma.Characters;
using BG_Koakuma.Powers;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Relics;

public abstract class KoakumaRelic : ModRelicTemplate
{
    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"{Entry.ResPath}/images/relics/outline/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/relics/big/{GetType().Name}.png");

    protected static DynamicVar AmountVar(string name, decimal value) => new(name, value);

    protected static DynamicVar MagicVar(string name, decimal value) =>
        SecondaryResourceVars.For(name, KoakumaMagic.MagicId, value);

    protected int Amount(string name) => DynamicVars[name].IntValue;

    protected virtual IEnumerable<string> ExtraKoakumaHoverTipIds => [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        KoakumaHoverTips.CreateMany(ExtraKoakumaHoverTipIds);

    protected async Task GainMagic(PlayerChoiceContext choiceContext, int amount)
    {
        await KoakumaMechanics.GainMagic(Owner, amount, this);
    }
}
