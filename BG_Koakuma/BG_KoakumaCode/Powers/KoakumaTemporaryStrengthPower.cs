using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using BG_Koakuma.Cards;
using BG_Koakuma.Tooltips;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace BG_Koakuma.Powers;

[RegisterPower]
public sealed class KoakumaTemporaryStrengthPower : KoakumaTemporaryAppliedPower<BG_KoakumaRubyBook, StrengthPower>
{
    protected override IEnumerable<IHoverTip> ExtraKoakumaHoverTips => [PowerTip<StrengthPower>()];

}
