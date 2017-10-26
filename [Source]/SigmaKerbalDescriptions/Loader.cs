using System.Linq;
using UnityEngine;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KerbalDescriptionsLoader : MonoBehaviour
    {
        void Start()
        {
            ConfigNode[] nodes = GameDatabase.Instance?.GetConfigNodes("SigmaKerbalDescriptions")?.FirstOrDefault(n => n.HasNode("Information"))?.GetNodes("Information");

            for (int i = 0; i < nodes?.Length; i++)
            {
                ConfigNode[] requirements = nodes[i].GetNodes("Requirements");
                ConfigNode text = nodes[i].GetNode("Text");

                if (requirements.Length == 0)
                    requirements = new[] { new ConfigNode() };

                for (int j = 0; j < requirements.Length; j++)
                {
                    Information.DataBase.Add(new Information(requirements[j], text));
                }
            }

            if (Information.DataBase?.Count > 0)
                Information.OrderDB();
        }
    }
}
