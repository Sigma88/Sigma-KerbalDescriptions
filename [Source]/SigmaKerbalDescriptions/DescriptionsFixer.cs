using System.Linq;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
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
            Description.UpdateAll(HighLogic.CurrentGame.CrewRoster);
        }

        void Update()
        {
            if (count < 3)
            {
                count++;

                if (count == 3)
                {
                    Debug.Log("DescriptionsFixer", "Update");
                    Description.UpdateAll(HighLogic.CurrentGame.CrewRoster);
                    DestroyImmediate(this);
                }
            }
        }
    }

    internal static class Description
    {
        internal static void UpdateAll(KerbalRoster kerbals)
        {
            for (int i = 0; i < kerbals?.Count; i++)
            {
                Update(kerbals[i]);
            }
        }

        internal static void UpdateAll(ProtoCrewMember[] kerbals)
        {
            for (int i = 0; i < kerbals?.Length; i++)
            {
                Update(kerbals[i]);
            }
        }

        internal static void Update(ProtoCrewMember kerbal)
        {
            Debug.Log("Description.Update", "kerbal = " + kerbal);
            if (kerbal == null) return;

            CrewListItem item = kerbal.crewListItem();
            Debug.Log("Description.Update", "item = " + item);
            TooltipController_CrewAC tooltip = item.GetTooltip();
            Debug.Log("Description.Update", "tooltip = " + tooltip);

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
                bool keepAddingText = true;
                bool tooltipNameChanged = false;
                bool itemNameChanged = false;
                bool spriteChanged = false;

                for (int i = 0; i < Information.DataBase?.Count; i++)
                {
                    Information info = Information.DataBase[i];

                    if (info.index == index || info.index == null)
                    {
                        string[] text = info.GetText(kerbal);

                        if (text.Any() && keepAddingText)
                        {
                            if (info.useChance != 1 && indexChance == null)
                                indexChance = kerbal.Hash(info.useGameSeed) % 100;

                            if (info.useChance == 1 || indexChance < info.useChance * 100)
                            {
                                if (info.unique)
                                    description = "";

                                if (info.unique || info.last)
                                    keepAddingText = false;

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
                            tooltip.titleString = Information.newTooltipName.PrintFor(kerbal);
                            tooltipNameChanged = true;
                        }

                        if (item != null && !itemNameChanged && !string.IsNullOrEmpty(Information.newItemName))
                        {
                            item.kerbalName.text = Information.newItemName.PrintFor(kerbal);
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
                    tooltip.descriptionString = description.PrintFor(kerbal);

                    if (kerbal.type == ProtoCrewMember.KerbalType.Applicant)
                        tooltip.descriptionString += CheckForErrors();

                    UIMasterController.Instance.DespawnTooltip(tooltip);
                }
            }
        }

        private static string CheckForErrors()
        {
            AstronautComplex complex = Resources.FindObjectsOfTypeAll<AstronautComplex>()?.FirstOrDefault();
            KerbalRoster roster = HighLogic.CurrentGame?.CrewRoster;
            if (complex == null || roster == null) return "";

            int? active = roster?.GetActiveCrewCount();

            if (active < complex?.crewLimit())
            {
                if (GameVariables.Instance?.GetRecruitHireCost((int)active) > Funding.Instance?.Funds)
                {
                    return TooltipErrors.OutOfFunds;
                }
            }
            else
            {
                return TooltipErrors.AtCapacity;
            }

            return "";
        }
    }
}
