using System.Linq;
using UnityEngine;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KerbalDescriptionsLoader : MonoBehaviour
    {
        void Start()
        {
            ConfigNode[] SKD = GameDatabase.Instance.GetConfigNodes("SigmaKerbalDescriptions");
            ConfigNode[] nodes = SKD?.FirstOrDefault(n => n.HasNode("Information")).GetNodes("Information");

            for (int i = 0; i < nodes?.Length; i++)
            {
                ConfigNode[] requirements = nodes[i].GetNodes("Requirements");
                ConfigNode text = nodes[i].GetNode("Text");

                if (requirements.Length == 0)
                    requirements = new[] { new ConfigNode() };

                for (int j = 0; j < requirements.Length; j++)
                {
                    Information.DataBase.Add(new Information(requirements[j], text));
                    Debug.Log("SigmaLog: added Information = " + Information.DataBase.LastOrDefault());
                    Debug.Log("SigmaLog: added count = " + Information.DataBase?.Count);
                    Debug.Log("SigmaLog: displayName = " + Information.DataBase.LastOrDefault()?.displayName);
                    Debug.Log("SigmaLog: informations = " + Information.DataBase.LastOrDefault()?.informations?.Length);
                }
            }

            Information.OrderDB();
        }
    }
}
