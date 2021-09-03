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
        private static string _VERSION = "0.9.0";

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
            this.name = "English Patch Mod";
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }

    /*
    [BepInPlugin("plugins.test1", "Test Mod 1", "0.1.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class TestMod1 : BaseUnityPlugin, IPoWMod
    {
        public string GetVersion()
        {
            return "0.1.0";
        }

        public string GetName()
        {
            return "Test Mod 1";
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        private void Awake()
        {
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }

    [BepInPlugin("plugins.test2", "Test Mod 2", "0.2.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class TestMod2 : BaseUnityPlugin, IPoWMod
    {
        public string GetVersion()
        {
            return "0.2.0";
        }

        public string GetName()
        {
            return "Test Mod 2";
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        private void Awake()
        {
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }

    [BepInPlugin("plugins.test3", "Test Mod 3", "0.3.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class TestMod3 : BaseUnityPlugin, IPoWMod
    {
        public string GetVersion()
        {
            return "0.3.0";
        }

        public string GetName()
        {
            return "Test Mod 3";
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        private void Awake()
        {
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }

    [BepInPlugin("plugins.test4", "Test Mod 4", "0.4.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class TestMod4 : BaseUnityPlugin, IPoWMod
    {
        public string GetVersion()
        {
            return "0.4.0";
        }

        public string GetName()
        {
            return "Test Mod 4";
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        private void Awake()
        {
            //Adding it to ModAPI list
            ModAPI.ModAPI.GetInstance().AddMod(this);
        }
    }
    */
}