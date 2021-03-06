﻿using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
using KSP.UI.TooltipTypes;


namespace SigmaKerbalDescriptions
{
    static class Extensions
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

        internal static int crewLimit(this AstronautComplex complex)
        {
            FieldInfo crewLimit = typeof(AstronautComplex).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(k => k.Name == "crewLimit");

            try
            {
                return (int)crewLimit.GetValue(complex);
            }
            catch
            {
                return int.MaxValue;
            }
        }

        internal static CrewListItem crewListItem(this ProtoCrewMember kerbal)
        {
            AstronautComplex ac = Resources.FindObjectsOfTypeAll<AstronautComplex>().FirstOrDefault();
            Debug.Log("Kerbal.crewListItem", "AstronautComplex = " + ac);
            if (ac == null) return null;

            UIList[] list = new[] { ac?.ScrollListApplicants, ac?.ScrollListAssigned, ac?.ScrollListAvailable, ac?.ScrollListKia };
            CrewListItem item = null;

            for (int i = 0; i < list?.Length; i++)
            {
                var items = list[i]?.GetUiListItems();
                for (int j = 0; j < items?.Count; j++)
                {
                    item = items[j]?.GetComponent<CrewListItem>();
                    if (item.GetCrewRef() == kerbal)
                        return item;
                }
            }

            Debug.Log("Kerbal.crewListItem", "Item = " + item);
            return null;
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
                .Replace("&seed;", "" + HighLogic.CurrentGame.Seed)
                .Replace("&visited;", "" + (kerbal?.careerLog?.Entries?.Select(e => e.target)?.Where(t => !string.IsNullOrEmpty(t))?.Distinct()?.Count() ?? 0))
                .Replace("&missions;", "" + (kerbal?.careerLog?.Entries?.Select(e => e.flight)?.Distinct()?.Count() ?? 0))
                .GetHashColor();
        }

        internal static string GetHashColor(this string s)
        {
            int start = 0;

            while (s.Substring(start).Contains("&Color"))
            {
                start = s.IndexOf("&Color");
                int end = s.Substring(start).IndexOf(";") + 1;
                if (end > 9)
                {
                    int add = 0;
                    switch (s.Substring(start + 6, 2))
                    {
                        case "Lo":
                            break;
                        case "Hi":
                            add = 80;
                            break;
                        default:
                            start++;
                            continue;
                    }
                    string text = s.Substring(start, end);
                    int hash = Math.Abs(text.GetHashCode());
                    string color = "#";
                    for (int i = 0; i < 3; i++)
                    {
                        color += (hash % 176 + add).ToString("X");
                        hash = Math.Abs(hash.ToString().GetHashCode());
                    }
                    s = s.Replace(text, "<color=" + color + ">");
                }
                else
                {
                    continue;
                }
            }
            return s;
        }
    }
}
