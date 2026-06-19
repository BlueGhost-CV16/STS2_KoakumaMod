using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class MagicPagesPower : KoakumaPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterRead(PlayerChoiceContext choiceContext, CardModel? readCard)
    {
        var drawPile = PileType.Draw.GetPile(Owner.Player);
        var bottom = drawPile.Cards.LastOrDefault();
        if (bottom != null)
        {
            await CardPileCmd.Add(bottom, PileType.Hand, CardPilePosition.Bottom);
        }
        await PowerCmd.Decrement(this);
    }
}
