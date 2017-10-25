using System;
using System.Collections.Generic;
using System.Linq;
using Gender = ProtoCrewMember.Gender;


namespace SigmaKerbalDescriptions
{
    public class Information
    {
        public static string hash = "";
        public static int? indexChance = null;
        public static string newName = null;
        public static List<Information> DataBase = new List<Information>();
        public static List<Information> WithName = new List<Information>();
        public static List<Information> NoName = new List<Information>();

        public string name = null;
        public int? index = null;

        public float useChance = 1;
        public Gender? gender = null;
        public string[] trait = null;
        public bool? veteran = null;
        public bool? isBadass = null;
        public int minLevel = 0;
        public int maxLevel = 5;
        public float minCourage = 0;
        public float maxCourage = 1;
        public float minStupidity = 0;
        public float maxStupidity = 1;

        public string displayName = null;
        public string[] informations = new string[] { };


        public string[] GetNext(ProtoCrewMember kerbal)
        {
            if (gender == null || gender == kerbal.gender)
            {
                if (trait == null || trait.Contains(kerbal.trait))
                {
                    if (veteran == null || veteran == kerbal.veteran)
                    {
                        if (isBadass == null || isBadass == kerbal.isBadass)
                        {
                            if (minLevel <= kerbal.experienceLevel && maxLevel >= kerbal.experienceLevel)
                            {
                                if (minCourage <= kerbal.courage && maxCourage >= kerbal.courage)
                                {
                                    if (minStupidity <= kerbal.stupidity && maxStupidity >= kerbal.stupidity)
                                    {
                                        Information.newName = displayName;
                                        return informations;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return new string[] { };
        }


        public Information(ConfigNode requirements, ConfigNode text)
        {
            useChance = Parse(requirements.GetValue("useChance"), useChance);
            index = Parse(requirements.GetValue("index"), index);
            name = requirements.GetValue("name");
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

            Debug.Log("SigmaLog: about to parse displayName");
            displayName = text.GetValue("displayName");
            Debug.Log("SigmaLog: displayName = " + displayName);
            informations = text.GetValues("info")?.Where(s => !string.IsNullOrEmpty(s))?.ToArray();
        }


        float Parse(string s, float defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        float? Parse(string s, float? defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        bool Parse(string s, bool defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        bool? Parse(string s, bool? defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        int Parse(string s, int defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }
        int? Parse(string s, int? defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }

        Gender? Parse(string s, Gender? defaultValue)
        {
            try { return (Gender)Enum.Parse(typeof(Gender), s); }
            catch { return defaultValue; }
        }

        public static void OrderDB()
        {
            DataBase = DataBase.Where(i => i.informations.Length > 0 || !string.IsNullOrEmpty(i.displayName)).ToList();
            WithName = DataBase.Where(i => i.name != null).ToList();
            NoName = DataBase.Where(i => i.name == null && i.index != null).OrderBy(i => i.index).ThenBy(i => i.useChance).ToList();
            NoName.AddRange(DataBase.Where(i => i.name == null && i.index == null).OrderBy(i => i.index).ThenBy(i => i.useChance));
            Debug.Log("SigmaLog: " + DataBase.Count + " = " + WithName.Count + " + " + NoName.Count);
        }
    }
}
