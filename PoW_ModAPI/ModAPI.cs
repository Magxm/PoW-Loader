using BepInEx;
using BepInEx.Bootstrap;

using HarmonyLib;

using System.IO;

using UnityEngine;

namespace ModAPI
{
    [BepInDependency("gravydevsupreme.xunity.resourceredirector", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("plugins.modapi", "Mod API", "0.4.0")]
    [BepInProcess("PathOfWuxia.exe")]
    public class ModAPI : BaseUnityPlugin, PoWMod
    {
        private static string _VERSION = "0.4.0";

        public string GetVersion()
        {
            return _VERSION;
        }

        public static ModAPI GetInstance()
        {
            if (!Chainloader.PluginInfos.ContainsKey("plugins.modapi"))
            {
                Debug.LogError("Tried to obtain ModApi Instance but it was not loaded!");
                return null;
            }

            return Chainloader.PluginInfos["plugins.modapi"].Instance as ModAPI;
        }

        private Harmony _HM;
        public ResourceRedirectManager ResourceRedirector = ResourceRedirectManager.GetInstance();

        private void Awake()
        {
            this.name = "ModAPI";

            //Hooking
            _HM = new Harmony("ModAPI");
            _HM.PatchAll();

            ResourceRedirector.AddRessourceFolder("Mods" + Path.DirectorySeparatorChar + "ImageTest");

            //Initing in game console (basically just a log viewer right now)
            UI.Console.Init();
        }

        private void OnDestroy()
        {
            //Cleaning up in reverse order in which we set everything up.
            UI.Console.Unload();

            _HM.UnpatchAll("ModAPI");
        }
    }
}