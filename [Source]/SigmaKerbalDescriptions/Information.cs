using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gender = ProtoCrewMember.Gender;
using Type = ProtoCrewMember.KerbalType;


namespace SigmaKerbalDescriptions
{
    class Information
    {
        // Static
        internal static string hash = "";
        internal static string newItemName = null;
        internal static string newTooltipName = null;
        internal static Texture newSprite = null;
        internal static List<Information> List = new List<Information>();
        internal static List<Information> DataBase = new List<Information>();

        // Identifiers
        string name = null;
        internal int? index = null;

        // Requirements
        internal bool useGameSeed = false;
        internal bool unique = false;
        internal bool last = false;
        internal float useChance = 1;
        Type? rosterStatus = null;
        Gender? gender = null;
        string[] trait = null;
        bool? veteran = null;
        bool? isBadass = null;
        int minLevel = 0;
        int maxLevel = 5;
        float minCourage = 0;
        float maxCourage = 1;
        float minStupidity = 0;
        float maxStupidity = 1;

        // Define
        string displayName = null;
        string tooltipName = null;
        Texture sprite = null;
        string[] informations = new string[] { };


        // Get
        internal string[] GetText(ProtoCrewMember kerbal)
        {
            newTooltipName = null;
            newItemName = null;
            newSprite = null;

            if (name == null || name == kerbal.name)
            {
                Debug.Log("Information.GetText", "Matched name = " + name + " to kerbal name = " + kerbal.name);
                if (rosterStatus == null || rosterStatus == kerbal.type)
                {
                    Debug.Log("Information.GetText", "Matched rosterStatus = " + rosterStatus + " to kerbal rosterStatus = " + kerbal.type);
                    if (gender == null || gender == kerbal.gender)
                    {
                        Debug.Log("Information.GetText", "Matched gender = " + gender + " to kerbal gender = " + kerbal.gender);
                        if (trait == null || trait.Contains(kerbal.trait))
                        {
                            Debug.Log("Information.GetText", "Matched trait = " + trait + " to kerbal trait = " + kerbal.trait);
                            if (veteran == null || veteran == kerbal.veteran)
                            {
                                Debug.Log("Information.GetText", "Matched veteran = " + veteran + " to kerbal veteran = " + kerbal.veteran);
                                if (isBadass == null || isBadass == kerbal.isBadass)
                                {
                                    Debug.Log("Information.GetText", "Matched isBadass = " + isBadass + " to kerbal isBadass = " + kerbal.isBadass);
                                    if (minLevel <= kerbal.experienceLevel && maxLevel >= kerbal.experienceLevel)
                                    {
                                        Debug.Log("Information.GetText", "Matched minLevel = " + minLevel + ", maxLevel = " + maxLevel + " to kerbal level = " + kerbal.experienceLevel);
                                        if (minCourage <= kerbal.courage && maxCourage >= kerbal.courage)
                                        {
                                            Debug.Log("Information.GetText", "Matched minCourage = " + minCourage + ", maxCourage = " + maxCourage + " to kerbal courage = " + kerbal.courage);
                                            if (minStupidity <= kerbal.stupidity && maxStupidity >= kerbal.stupidity)
                                            {
                                                Debug.Log("Information.GetText", "Matched minStupidity = " + minStupidity + ", maxStupidity = " + maxStupidity + " to kerbal stupidity = " + kerbal.stupidity);

                                                newItemName = displayName;
                                                Debug.Log("Information.GetText", "newItemName = " + newItemName);
                                                newTooltipName = tooltipName;
                                                Debug.Log("Information.GetText", "newTooltipName = " + newTooltipName);
                                                newSprite = sprite;
                                                Debug.Log("Information.GetText", "newSprite = " + newSprite);

                                                Debug.Log("Information.GetText", "informations count = " + informations.Length);
                                                return informations;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new string[] { };
        }


        internal Information(ConfigNode requirements, ConfigNode text)
        {
            useGameSeed = Parse(requirements.GetValue("useGameSeed"), useGameSeed);
            unique = Parse(requirements.GetValue("unique"), unique);
            last = Parse(requirements.GetValue("last"), last);
            useChance = Parse(requirements.GetValue("useChance"), useChance);
            index = Parse(requirements.GetValue("index"), index);
            name = requirements.GetValue("name");
            rosterStatus = Parse(requirements.GetValue("rosterStatus"), rosterStatus);
            gender = Parse(requirements.GetValue("gender"), gender);
            trait = requirements.HasValue("trait") ? requirements.GetValues("trait") : null;
            veteran = Parse(requirements.GetValue("veteran"), veteran);
            isBadass = Parse(requirements.GetValue("isBadass"), isBadass);
            minLevel = Parse(requirements.GetValue("minLevel"), minLevel);
            maxLevel = Parse(requirements.GetValue("maxLevel"), maxLevel);
            minCourage = Parse(requirements.GetValue("minCourage"), minCourage);
            maxCourage = Parse(requirements.GetValue("maxCourage"), maxCourage);
            minStupidity = Parse(requirements.GetValue("minStupidity"), minStupidity);
            maxStupidity = Parse(requirements.GetValue("maxStupidity"), maxStupidity);

            displayName = text.GetValue("displayName");
            tooltipName = text.GetValue("tooltipName");
            sprite = Parse(text.GetValue("sprite"), sprite);
            informations = text.GetValues("info")?.Where(s => !string.IsNullOrEmpty(s))?.ToArray();
        }


        float Parse(string s, float defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        float? Parse(string s, float? defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        bool Parse(string s, bool defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        bool? Parse(string s, bool? defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        int Parse(string s, int defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }
        int? Parse(string s, int? defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }

        Type? Parse(string s, Type? defaultValue)
        {
            try { return (Type)Enum.Parse(typeof(Type), s); }
            catch { return defaultValue; }
        }

        Gender? Parse(string s, Gender? defaultValue)
        {
            try { return (Gender)Enum.Parse(typeof(Gender), s); }
            catch { return defaultValue; }
        }

        Texture Parse(string s, Texture defaultValue)
        {
            return Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == s) ?? defaultValue;
        }

        internal static void OrderDB()
        {
            Debug.Log("Information.OrderDB", "Total Information Nodes Loaded = " + List.Count);
            List = List.Where(i => i.informations.Length > 0 || !string.IsNullOrEmpty(i.tooltipName) || !string.IsNullOrEmpty(i.displayName) || i.sprite != null).ToList();
            Debug.Log("Information.OrderDB", "Valid Information Nodes Loaded = " + List.Count);
            Debug.Log("Information.OrderDB", "Initial DataBase count = " + DataBase.Count);
            DataBase.AddRange(List.Where(i => i.name != null && i.index != null).OrderBy(i => i.index).ThenBy(i => i.useChance));
            Debug.Log("Information.OrderDB", "Added withName, withIndex to DataBase count = " + DataBase.Count);
            DataBase.AddRange(List.Where(i => i.name != null && i.index == null).OrderBy(i => i.index).ThenBy(i => i.useChance));
            Debug.Log("Information.OrderDB", "Added withName, noIndex to DataBase count = " + DataBase.Count);
            DataBase.AddRange(List.Where(i => i.name == null && i.index != null).OrderBy(i => i.index).ThenBy(i => i.useChance));
            Debug.Log("Information.OrderDB", "Added noName, withIndex to DataBase count = " + DataBase.Count);
            DataBase.AddRange(List.Where(i => i.name == null && i.index == null).OrderBy(i => i.index).ThenBy(i => i.useChance));
            Debug.Log("Information.OrderDB", "Added noName, noIndex to DataBase count = " + DataBase.Count);
        }
    }
}
