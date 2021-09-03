using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;

using HarmonyLib;

using System.Collections.Generic;

using UnityEngine;

namespace ModAPI
{
    public class PoWMod_Wrapper
    {
        public IPoWMod Mod;
        public BaseUnityPlugin ModAsPlugin;

        private ConfigEntry<int> config_LoadOrderIndex;
        private ConfigEntry<bool> config_Enabled;

        public PoWMod_Wrapper(IPoWMod mod)
        {
            Mod = mod;
            ModAsPlugin = Mod as BaseUnityPlugin;

            config_LoadOrderIndex = ModAsPlugin.Config.Bind<int>("Generic", "LoadOrderIndex", -1, "Load order index");
            config_Enabled = ModAsPlugin.Config.Bind<bool>("Generic", "Enabled", true, "Should mod be loaded");
            ModAsPlugin.Config.Save();
        }

        public int GetLoadOrderIndex()
        {
            return config_LoadOrderIndex.Value;
        }

        public void SetLoadOrderIndex(int newValue)
        {
            config_LoadOrderIndex.Value = newValue;
            ModAsPlugin.Config.Save();
        }

        public bool IsEnabled()
        {
            return config_Enabled.Value;
        }
    }

    [BepInDependency("gravydevsupreme.xunity.resourceredirector", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("plugins.modapi", "Mod API", "0.4.0")]
    [BepInProcess("PathOfWuxia.exe")]
    public class ModAPI : BaseUnityPlugin
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

        private bool _ModsLoaded = false;
        private SortedList<int, PoWMod_Wrapper> _Mods = new SortedList<int, PoWMod_Wrapper>();
        private int _ModsHighestLoadOrderIndex = -1;
        private List<PoWMod_Wrapper> _NewMods = new List<PoWMod_Wrapper>();

        public void AddMod(IPoWMod mod)
        {
            if (_ModsLoaded)
            {
                Debug.LogError("The mod " + mod.GetName() + " was added after the mods have already been processed!");
            }
            else
            {
                Debug.Log("Mod found: " + mod.GetName());
            }

            var modWrapper = new PoWMod_Wrapper(mod);

            int loadOrderIndex = modWrapper.GetLoadOrderIndex();
            if (loadOrderIndex == -1)
            {
                _NewMods.Add(modWrapper);
            }
            else
            {
                if (loadOrderIndex > _ModsHighestLoadOrderIndex)
                    _ModsHighestLoadOrderIndex = loadOrderIndex;

                _Mods.Add(loadOrderIndex, modWrapper);
            }
        }

        public void LoadMods()
        {
            Debug.Log("Loading mods!");
            if (_ModsLoaded)
            {
                Debug.Log("Mods already loaded...");
                return;
            }

            //Processing new mods and giving them a l
            for (int i = 0; i < _NewMods.Count; ++i)
            {
                Debug.Log("Found new mod " + _NewMods[i].Mod.GetName());
                _ModsHighestLoadOrderIndex++;
                _NewMods[i].SetLoadOrderIndex(_ModsHighestLoadOrderIndex);
                _Mods.Add(_ModsHighestLoadOrderIndex, _NewMods[i]);
            }
            _NewMods.Clear();

            //Loading all mods
            for (int i = 0; i < _Mods.Count; ++i)
            {
                if (_Mods[i].IsEnabled())
                {
                    Debug.Log("Loading Mod " + _Mods[i].Mod.GetName());
                    _Mods[i].Mod.Load();
                }
            }

            _ModsLoaded = true;
        }

        private void Awake()
        {
            this.name = "ModAPI";

            //Hooking
            _HM = new Harmony("ModAPI");
            _HM.PatchAll();

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

    [HarmonyPatch(typeof(Heluo.Game), "OnBeforeSceneLoad")]
    public class Game_OnBeforeSceneLoad_Hook
    {
        public static bool Prefix()
        {
            Debug.Log("Game.OnBeforeSceneLoad called");
            ModAPI.GetInstance().LoadMods();

            //Call original
            return true;
        }
    }
}