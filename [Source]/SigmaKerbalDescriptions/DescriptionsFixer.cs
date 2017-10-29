using System.Linq;
using UnityEngine;
using KSP.UI;
using KSP.UI.TooltipTypes;


namespace SigmaKerbalDescriptions
{
    class DescriptionsFixer : MonoBehaviour
    {
        static int count = 0;

        void Start()
        {
            count = 0;
            Debug.Log("DescriptionsFixer", "Start");
            Description.UpdateAll(HighLogic.CurrentGame.CrewRoster, false);
        }

        void Update()
        {
            if (count < 2)
            {
                count++;

                if (count == 2)
                {
                    Debug.Log("DescriptionsFixer", "Update");
                    Description.UpdateAll(HighLogic.CurrentGame.CrewRoster, fixListItem: false);
                    DestroyImmediate(this);
                }
            }
        }
    }

    internal static class Description
    {
        internal static void UpdateAll(KerbalRoster kerbals, bool fixTooltip = true, bool fixListItem = true)
        {
            for (int i = 0; i < kerbals?.Count; i++)
            {
                Update(kerbals[i], fixTooltip, fixListItem);
            }
        }

        internal static void UpdateAll(ProtoCrewMember[] kerbals, bool fixTooltip = true, bool fixListItem = true)
        {
            for (int i = 0; i < kerbals?.Length; i++)
            {
                Update(kerbals[i], fixTooltip, fixListItem);
            }
        }

        internal static void Update(ProtoCrewMember kerbal, bool fixTooltip = true, bool fixListItem = true)
        {
            if (!fixTooltip && !fixListItem) return;
            Debug.Log("Description.Update", "kerbal = " + kerbal);
            if (kerbal == null) return;

            CrewListItem item = kerbal.crewListItem();
            Debug.Log("Description.Update", "item = " + item);
            TooltipController_CrewAC tooltip = item.GetTooltip();
            Debug.Log("Description.Update", "tooltip = " + tooltip);

            if (!fixListItem) item = null;
            if (!fixTooltip) tooltip = null;

            // Missing Kerbal Tooltip
            if (tooltip == null && item == null)
            {
                Debug.Log("Description.Update", "Couldn't find CrewListItem and Tooltip for Kerbal \"" + kerbal.name + "\".");
                return;
            }

            // Custom Kerbal ListItem and Tooltip
            else
            {
                Information.hash = "";
                string description = "";

                int index = 0;
                int? indexChance = null;
                bool tooltipNameChanged = false;
                bool itemNameChanged = false;
                bool spriteChanged = false;

                for (int i = 0; i < Information.DataBase?.Count; i++)
                {
                    Information info = Information.DataBase[i];

                    if (info.index == index || info.index == null)
                    {
                        string[] text = info.GetText(kerbal);

                        if (text.Any())
                        {
                            if (info.useChance != 1 && indexChance == null)
                                indexChance = kerbal.Hash(info.useGameSeed) % 100;

                            if (info.useChance == 1 || indexChance < info.useChance * 100)
                            {
                                if (text.Length == 1)
                                    description += text[0];
                                else
                                    description += text[kerbal.Hash(info.useGameSeed) % text.Length];

                                index++;
                                indexChance = null;
                            }
                        }

                        if (tooltip != null && !tooltipNameChanged && !string.IsNullOrEmpty(Information.newTooltipName))
                        {
                            tooltip.titleString = Information.newTooltipName.Replace("&name;", kerbal.name);
                            tooltipNameChanged = true;
                        }

                        if (item != null && !itemNameChanged && !string.IsNullOrEmpty(Information.newItemName))
                        {
                            item.kerbalName.text = Information.newItemName.Replace("&name;", kerbal.name);
                            itemNameChanged = true;
                        }

                        if (item != null && !spriteChanged && Information.newSprite != null)
                        {
                            item.kerbalSprite.texture = Information.newSprite;
                            spriteChanged = true;
                        }
                    }
                }

                if (tooltip != null && !string.IsNullOrEmpty(description))
                {
                    tooltip.descriptionString = description.Replace("&br;", "\n");

                    UIMasterController.Instance.DespawnTooltip(tooltip);
                }
            }
        }
    }
}
