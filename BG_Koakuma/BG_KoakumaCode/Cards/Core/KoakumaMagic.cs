using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;

namespace BG_Koakuma.Cards;

internal static class KoakumaMagic
{
    public static SecondaryResourceDefinition MagicDefinition { get; private set; } = null!;
    public const string LocalId = "magic";
    public static string MagicId { get; private set; } = ModSecondaryResourceRegistry.GetResourceId(Entry.ModId, LocalId);

    public static void Register()
    {
        var registry = RitsuLibFramework.GetSecondaryResourceRegistry(Entry.ModId);
        MagicDefinition = registry.Register(LocalId, new SecondaryResourceDefinition(
            defaultAmount: 0,
            minAmount: 0,
            persistencePolicy: SecondaryResourcePersistencePolicy.Combat,
            smallIconPath: $"{Entry.ResPath}/images/characters/Magic_small.png",
            largeIconPath: $"{Entry.ResPath}/images/characters/Magic.png"
            ));
        MagicId = MagicDefinition.Id;
        SecondaryResourceHook.RegisterGlobalListener(OptionalMagicCostListener.Instance);
        
        // 战斗计数器。使用的图标就是你注册时提供的图标
        registry.RegisterCombatUi(
            "magic_combat_counter",
            parent =>
            {
                var row = NSecondaryResourceCounter.Create(MagicDefinition, new SecondaryResourceCounterStyle
                {
                    FontSize = 32,
                    PositiveColor = Colors.LightPink,
                    ZeroColor = Colors.LightPink,
                    OutlineColor = Colors.Black,
                    OutlineSize = 12,
                    AmountLabelOffset = new Vector2(9,
                        8),
                    RowSeparation = 0,
                    FormatAmount = (amount,
                        max) => amount.ToString(),
                    GainFeedback = SecondaryResourceCounterGainFeedback.StarCounterLike,
                    IconStyle = SecondaryResourceIconStyle.Default with
                    {
                        Size = new Vector2(64,
                            64),
                        HoverTip = SecondaryResourceHoverTipStyle.Default,
                    },

                });
                // 自由指定位置。例如这里我们找到能量计数器的位置，放在它旁边
                var energyCounter = parent.GetNode<Control>("%EnergyCounterContainer");
                row.Position = energyCounter.Position + new Vector2(80, 72);
                return row;
            },
            ctx => ctx.Node.Bind(ctx.Player)
        );

        // 卡牌面上的次级资源费用显示。使用的图标就是你注册时提供的图标
        registry.RegisterCardUi(
            "magic_card_ui",
            parent =>
            {
                var ui = NSecondaryResourceCardCostUi.Create(MagicId, new SecondaryResourceCardCostUiStyle
                {
                    IconSize = new Vector2(56, 56),
                    LabelOffset = new Vector2(4, 4),
                    ReserveVanillaStarCostSlot = true,
                    FontSize = 32,
                    OutlineSize = 12,
                    AffordableColor = Colors.LightPink,
                    //UnaffordableColor = Colors.Red,
                    AffordableOutlineColor = Colors.Black,
                    //UnaffordableOutlineColor = Colors.DarkRed,
                    ExpandMode = TextureRect.ExpandModeEnum.KeepSize,
                    StretchMode = TextureRect.StretchModeEnum.Scale,
                    FormatCost = null,
                });
                // 自由指定位置。例如这里我们找到能量图标的位置，放在它旁边
                var energyIcon = parent.GetNode<TextureRect>("%EnergyIcon");
                ui.Position = energyIcon.Position + new Vector2(0, 60);
                return ui;
            },
            ctx => ctx.Node.Refresh(ctx)
        );
    }

    public static int Get(Player player)
    {
        return SecondaryResourceCmd.Get(player, MagicId);
    }

    public static Task<int> Gain(Player player, int amount, AbstractModel? source = null)
    {
        return SecondaryResourceCmd.Gain(player, MagicId, amount, source);
    }

    public static Task<int> Lose(Player player, int amount, AbstractModel? source = null)
    {
        return SecondaryResourceCmd.Lose(player, MagicId, amount, source);
    }

    public static Task<bool> Spend(Player player, int amount, CardModel? card = null, AbstractModel? source = null)
    {
        return SecondaryResourceCmd.Spend(player, MagicId, amount, card, source);
    }

    public static bool IsMagic(SecondaryResourceDefinition definition)
    {
        return string.Equals(definition.Id, MagicId, StringComparison.OrdinalIgnoreCase);
    }

}
