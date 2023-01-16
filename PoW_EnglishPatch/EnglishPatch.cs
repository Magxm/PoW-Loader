using System.IO;

using BepInEx;

using HarmonyLib;

using ModAPI;

namespace EnglishPatch
{
    [BepInPlugin("plugins.englishpatch", "English Patch", "0.9.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class EnglishPatch : BaseUnityPlugin, IPoWMod
    {
        private static readonly string _VERSION = "0.9.0";

        public string GetVersion()
        {
            return _VERSION;
        }

        public string GetName()
        {
            return "English Patch";
        }

        private Harmony _HM;

        public void Load()
        {
            ModAPI.ModAPI.GetInstance().ResourceRedirector.AddRessourceFolder("ModResources" + Path.DirectorySeparatorChar + "EnglishTranslate");

            _HM = new Harmony("EnglishPatch");
            _HM.PatchAll();
        }

        public void Unload()
        {
            _HM.UnpatchAll("EnglishPatch");
        }

        private void Awake()
        {
            this.name = GetName();
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }
}