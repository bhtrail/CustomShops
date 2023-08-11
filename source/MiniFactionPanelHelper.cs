using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;

using UnityEngine.UI;

namespace CustomShops;

public class MiniFactionPanelHelper
{
    public SG_Stores_MiniFactionWidget Widget { get; private set; }
    public Image FactionIcon { get; private set; }
    public HBSTooltip FactionTooltip { get; private set; }
    public SGReputationRatingIcon ratingIcon { get; private set; }
    public LocalizableText ReputationBonusText { get; private set; }

    public MiniFactionPanelHelper(SG_Stores_MiniFactionWidget widget)
    {
        Widget = widget;
        FactionIcon = Widget.FactionIcon;
        FactionTooltip = widget.FactionTooltip;
        ratingIcon = widget.ratingIcon;
        ReputationBonusText = widget.ReputationBonusText;
    }

    public void HideRatingIcons()
    {
        ratingIcon.HideAllIcons();
    }
}
