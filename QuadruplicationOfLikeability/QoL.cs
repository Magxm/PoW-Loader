using System.IO;

using BepInEx;

using HarmonyLib;

using ModAPI;

namespace QoL
{
    [BepInPlugin("plugins.quadruplicationoflikeability", "Quadruplication Of Likeability", "0.1.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class QoLMod : BaseUnityPlugin, IPoWMod
    {
        private static string _VERSION = "0.9.0";

        public string GetVersion()
        {
            return _VERSION;
        }

        public string GetName()
        {
            return "Quadruplication Of Likeability";
        }

        private Harmony _HM;

        public void Load()
        {
            _HM = new Harmony("QoL");
            _HM.PatchAll();
        }

        public void Unload()
        {
            _HM.UnpatchAll("QoL");
        }

        private void Awake()
        {
            this.name = GetName();
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }
}