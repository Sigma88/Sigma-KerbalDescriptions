using System.Linq;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.TooltipTypes;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class TooltipsChecker : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this);
            GameEvents.onGUIAstronautComplexSpawn.Add(Fix);
            GameEvents.onKerbalAdded.Add(Fix);
        }

        void Fix()
        {
            TooltipsFixer.skip1 = false;
            TooltipsFixer.skip2 = 0;
        }

        void Fix(ProtoCrewMember kerbal)
        {
            Fix();
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class TooltipsFixer : MonoBehaviour
    {
        // No idea why I need 2. But I do.
        public static bool skip1 = false;
        public static int skip2 = 0;

        void Update()
        {
            if
            (
                Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault()?.ScrollListApplicants?.isActiveAndEnabled == true &&
                Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault()?.ScrollListAssigned?.isActiveAndEnabled == true &&
                Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault()?.ScrollListAvailable?.isActiveAndEnabled == true &&
                Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault()?.ScrollListKia?.isActiveAndEnabled == true && !skip1
            )
            {
                skip1 = true;
            }

            if (skip1 && skip2 < 2)
            {
                skip2++;

                for (int i = 0; i < HighLogic.CurrentGame?.CrewRoster?.Count; i++)
                {
                    UpdateTooltip(HighLogic.CurrentGame.CrewRoster[i]);
                }
            }
        }

        void UpdateTooltip(ProtoCrewMember kerbal)
        {
            bool itemNameChanged = false;
            bool tooltipNameChanged = false;

            TooltipController_CrewAC tooltip = Resources.FindObjectsOfTypeAll<TooltipController_CrewAC>().FirstOrDefault(t => t?.titleString?.Contains(kerbal.name) == true);

            // Missing Kerbal Tooltip
            if (tooltip == null) return;

            // Custom Kerbal Tooltip
            else if (Information.WithName.Any(k => k.name == kerbal.name))
            {
                Information info = Information.WithName.FirstOrDefault(k => k.name == kerbal.name && (k.informations.Any(s => !string.IsNullOrEmpty(s)) || !string.IsNullOrEmpty(k.displayName)));

                if (info?.informations?.Any(s => !string.IsNullOrEmpty(s)) == true)
                    tooltip.descriptionString = info?.informations?.FirstOrDefault(s => !string.IsNullOrEmpty(s)).Replace("&br;", "\n");

                if (!tooltipNameChanged && !string.IsNullOrEmpty(info.tooltipName))
                {
                    tooltip.titleString = info.tooltipName.Replace("&name;", kerbal.name);
                    tooltipNameChanged = true;
                }

                if (!itemNameChanged && !string.IsNullOrEmpty(info.displayName))
                {
                    CrewListItem item = Resources.FindObjectsOfTypeAll<CrewListItem>().FirstOrDefault(it => it?.kerbalName?.text == kerbal.name);
                    if (item?.kerbalName != null)
                    {
                        item.kerbalName.text = info.displayName.Replace("&name;", kerbal.name);
                        itemNameChanged = true;
                    }
                }
            }

            // Random Kerbal Tooltip
            else
            {
                Information.hash = "";
                int index = 0;
                string description = "";

                for (int i = 0; i < Information.NoName?.Count; i++)
                {
                    Information info = Information.NoName[i];

                    if (info.index == index || info.index == null)
                    {
                        string[] text = info.GetNext(kerbal);

                        if (text.Any() || !string.IsNullOrEmpty(Information.newTooltipName) || !string.IsNullOrEmpty(Information.newItemName))
                        {
                            if (info.useChance != 1 && Information.indexChance == null)
                                Information.indexChance = kerbal.Hash() % 100;

                            if (info.useChance == 1 || Information.indexChance < info.useChance * 100)
                            {
                                if (text.Any())
                                {
                                    if (text.Length == 1)
                                        description += text[0];
                                    else
                                        description += text[kerbal.Hash() % text.Length];

                                    index++;
                                    Information.indexChance = null;
                                }

                                if (!tooltipNameChanged && !string.IsNullOrEmpty(Information.newTooltipName))
                                {
                                    tooltip.titleString = Information.newTooltipName.Replace("&name;", kerbal.name);
                                    tooltipNameChanged = true;
                                }

                                if (!itemNameChanged && !string.IsNullOrEmpty(Information.newItemName))
                                {
                                    CrewListItem item = Resources.FindObjectsOfTypeAll<CrewListItem>().FirstOrDefault(it => it?.kerbalName?.text == kerbal.name);
                                    if (item?.kerbalName != null)
                                    {
                                        item.kerbalName.text = Information.newItemName.Replace("&name;", kerbal.name);
                                        itemNameChanged = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(description))
                    tooltip.descriptionString = description.Replace("&br;", "\n");

                UIMasterController.Instance.DespawnTooltip(tooltip);
            }
        }
    }
}
