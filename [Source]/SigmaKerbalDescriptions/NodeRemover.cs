using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace SigmaKerbalDescriptions
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class ConfigNodeRemover : MonoBehaviour
    {
        static string folder = "GameData/Sigma/KerbalDescriptions/";
        static string file = "Settings";
        static string node = "SigmaKerbalDescriptions";

        void Awake()
        {
            string path = Assembly.GetExecutingAssembly().Location.Replace('\\', '/');
            while (path.Substring(1).Contains("GameData/"))
                path = path.Substring(1 + path.Substring(1).IndexOf("GameData/"));
            if (path != folder + "Plugins/" + Path.GetFileName(path))
                UnityEngine.Debug.Log(Debug.Tag + " WARNING: Incorrect plugin location => " + path);

            if (!Directory.Exists(folder))
            {
                UnityEngine.Debug.Log(Debug.Tag + " WARNING: Missing folder => " + folder);
                return;
            }

            if (!File.Exists(folder + file + ".cfg"))
            {
                UnityEngine.Debug.Log(Debug.Tag + " WARNING: Missing file => " + folder + file + ".cfg");

                File.WriteAllLines(folder + file + ".cfg", new[] { node + " {}" });
                return;
            }

            if (ConfigNode.Load(folder + file + ".cfg")?.HasNode(node) != true)
            {
                UnityEngine.Debug.Log(Debug.Tag + " WARNING: Missing node => " + folder + file + "/" + node);

                File.AppendAllText(folder + file + ".cfg", "\r\n" + node + " {}");
            }
        }

        void Start()
        {
            var configs = GameDatabase.Instance.GetConfigs(node).Where(c => c.url != (folder.Substring(9) + file + "/" + node)).ToArray();

            for (int i = 0; i < configs?.Length; i++)
            {
                configs[i].parent.configs.Remove(configs[i]);
            }
        }
    }
}
