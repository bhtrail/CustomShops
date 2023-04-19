using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;

using UnityEngine.UI;

namespace CustomShops
{
    public class MiniFactionPanelHelper
    {
        public SG_Stores_MiniFactionWidget Widget { get; private set; }
        //public Traverse MainTraverse { get; private set; }

        public Image FactionIcon { get; private set; }
        public HBSTooltip FactionTooltip { get; private set; }
        public SGReputationRatingIcon ratingIcon { get; private set; }
        public LocalizableText ReputationBonusText { get; private set; }

        public MiniFactionPanelHelper(SG_Stores_MiniFactionWidget widget)
        {
            Widget = widget;
            //MainTraverse = new Traverse(widget);
            //FactionIcon = MainTraverse.Field<Image>("FactionIcon").Value;
            FactionIcon = Widget.FactionIcon;
            //FactionTooltip = MainTraverse.Field<HBSTooltip>("FactionTooltip").Value;
            FactionTooltip = widget.FactionTooltip;
            //ratingIcon = MainTraverse.Field<SGReputationRatingIcon>("ratingIcon").Value;
            ratingIcon = widget.ratingIcon;
            //ReputationBonusText = MainTraverse.Field<LocalizableText>("ReputationBonusText").Value;
            ReputationBonusText = widget.ReputationBonusText;
        }

        public void HideRatingIcons()
        {
            //var tr = new Traverse(ratingIcon);
            //tr.Method("HideAllIcons").GetValue();
            ratingIcon.HideAllIcons();
        }
    }
}
