
using System.IO;

using BepInEx;
using BepInEx.Bootstrap;

using HarmonyLib;

using UnityEngine;

namespace ModAPI
{
    [BepInDependency("gravydevsupreme.xunity.resourceredirector", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("plugins.modapi", "Mod API", "1.0.0.0")]
    [BepInProcess("PathOfWuxia.exe")]

    public class ModAPI : BaseUnityPlugin, PoWMod
    {
        private static string _VERSION = "0.3.2";
        public string GetVersion()
        {
            return _VERSION;
        }

        public static ModAPI GetInstance()
        {
            if (!Chainloader.PluginInfos.ContainsKey("plugins.modapi"))
            {
                Debug.LogError("Tried to optain ModApi Instance but it was not loaded!");
                return null;
            }

            return Chainloader.PluginInfos["plugins.modapi"].Instance as ModAPI;
        }


        private Harmony _HM;
        public ResourceRedirectManager ResourceRedirector = ResourceRedirectManager.GetInstance();
        void Awake()
        {
            this.name = "ModAPI";

            //Hooking
            //_HM = new Harmony("ModAPI");
            //_HM.PatchAll();

            ResourceRedirector.AddRessourceFolder("Mods" + Path.DirectorySeparatorChar + "ImageTest");

            //Initing in game console (basically just a log viewer right now)
            UI.Console.Init();
        }

        void OnDestroy()
        {
            //Cleaning up in reverse order in which we set everything up.
            UI.Console.Unload();

            _HM.UnpatchAll("ModAPI");
        }
    }
}
