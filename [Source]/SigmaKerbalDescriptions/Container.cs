using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens.SpaceCenter.MissionSummaryDialog;
using TMPro;


namespace SigmaKerbalDescriptions
{
    class ListItemContainer
    {
        CrewListItem listItem = null;
        CrewWidget widget = null;

        internal string name
        {
            set
            {
                if (listItem != null)
                {
                    listItem.kerbalName.text = value;
                }
                else if (widget != null)
                {
                    FieldInfo header = typeof(CrewWidget).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(k => k.Name == "header");
                    TextMeshProUGUI displayName = (TextMeshProUGUI)header.GetValue(widget);
                    displayName.text = value;
                }
            }
        }

        internal Texture sprite
        {
            set
            {
                if (listItem != null)
                    listItem.kerbalSprite.texture = value;
                else if (widget != null)
                    widget.crewIcon.sprite = Sprite.Create((Texture2D)value, widget.crewIcon.sprite.rect, widget.crewIcon.sprite.pivot);
            }
        }

        internal ListItemContainer(CrewListItem listItem)
        {
            this.listItem = listItem;
        }

        internal ListItemContainer(CrewWidget widget)
        {
            this.widget = widget;
        }
    }
}
