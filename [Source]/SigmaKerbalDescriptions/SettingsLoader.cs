using UnityEngine;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class SettingsLoader : MonoBehaviour
    {
        void Start()
        {
            // Debug Spam
            if (bool.TryParse(UserSettings.ConfigNode?.GetValue("debug"), out bool debug) && debug) Debug.debug = true;

            // User Settings
            ConfigNode[] InfoNodes = UserSettings.ConfigNode.GetNodes("Information");

            for (int i = 0; i < InfoNodes?.Length; i++)
            {
                ConfigNode[] requirements = InfoNodes[i].GetNodes("Requirements");
                ConfigNode text = InfoNodes[i].GetNode("Text");

                if (requirements.Length == 0)
                    requirements = new[] { new ConfigNode() };

                for (int j = 0; j < requirements.Length; j++)
                {
                    Information.List.Add(new Information(requirements[j], text));
                }
            }

            if (Information.List?.Count > 0)
                Information.OrderDB();
        }
    }
}
