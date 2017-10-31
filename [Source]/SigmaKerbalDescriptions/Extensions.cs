using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.TooltipTypes;


namespace SigmaKerbalDescriptions
{
    internal static class Extensions
    {
        internal static int Hash(this ProtoCrewMember kerbal, bool useGameSeed)
        {
            string hash = Information.hash;

            if (string.IsNullOrEmpty(hash)) hash = kerbal.name;

            int h = Math.Abs(hash.GetHashCode());

            if (useGameSeed) h += Math.Abs(HighLogic.CurrentGame.Seed);

            Information.hash = h.ToString();

            return h;
        }

        internal static CrewListItem crewListItem(this ProtoCrewMember kerbal)
        {
            FieldInfo crew = typeof(CrewListItem).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(k => k.FieldType == typeof(ProtoCrewMember));
            Debug.Log("Kerbal.crewListItem", "Kerbal reflection = " + crew);
            AstronautComplex ac = Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault();
            Debug.Log("Kerbal.crewListItem", "AstronautComplex = " + ac);
            if (crew == null || ac == null) return null;

            UIList[] list = new[] { ac?.ScrollListApplicants, ac?.ScrollListAssigned, ac?.ScrollListAvailable, ac?.ScrollListKia };
            CrewListItem item = null;

            for (int i = 0; i < list?.Length; i++)
            {
                item = list[i]?.GetUiListItems()?.FirstOrDefault(k => crew.GetValue(k.GetComponent<CrewListItem>()) == kerbal)?.GetComponent<CrewListItem>();
                if (item != null) break;
            }

            Debug.Log("Kerbal.crewListItem", "Item = " + item);
            return item;
        }

        internal static TooltipController_CrewAC GetTooltip(this CrewListItem listItem)
        {
            return listItem?.GetComponent<TooltipController_CrewAC>();
        }

        internal static string PrintFor(this string s, ProtoCrewMember kerbal)
        {
            return s
                .Replace("&br;", "\n")
                .Replace("&name;", kerbal.name)
                .Replace("&trait;", kerbal.trait)
                .Replace("&seed;", HighLogic.CurrentGame.Seed)
                .Replace("&visited;", "" + (kerbal?.careerLog?.Entries?.Select(e => e.target)?.Distinct()?.Count() ?? 0))
                .Replace("&missions;", "" + (kerbal?.careerLog?.Entries?.Select(e => e.flight)?.Distinct()?.Count() ?? 0));
        }
    }
}
