using System.IO;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using ModAPI;
using UnityEngine;

namespace QoL
{
    [BepInPlugin("plugins.quadruplicationoflikeability", "Quadruplication Of Likeability", "0.2.0")]
    [BepInDependency("plugins.modapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("PathOfWuxia.exe")]
    public class QoLMod : BaseUnityPlugin, IPoWMod
    {
        private static readonly string _VERSION = "0.2.0";

        private readonly ConfigEntry<float> Config_BattleAnimationSpeed;
        private readonly ConfigEntry<float> Config_BattleMovementSpeed;
        private readonly ConfigEntry<bool> Config_SmoothBattles;
        private readonly ConfigEntry<bool> Config_ShowExactRelationshipStatus;
        private readonly ConfigEntry<bool> Config_ShowGiftEffects;
        private readonly ConfigEntry<bool> Config_SortGifts;

        public QoLMod()
        {
            //QoLs
            Config_BattleAnimationSpeed = Config.Bind<float>("QoL", "Battle Animation Speed", 1.5f, "The speed at which game animations are played in battle. (Game Default is 1.0, Mod Default is 1.5)");
            Config_BattleMovementSpeed = Config.Bind<float>("QoL", "Battle Movement Speed", 1.5f, "The speed at which characters move in battle. (Game Default is 1.0, Mod Default is 1.5)");
            Config_SmoothBattles = Config.Bind<bool>("QoL", "Smooth Battles", true, "If enabled, Game animations, Transitions and movement is aggressively smoothed out. (Default is true)");

            //Cheaty QoLs
            Config_ShowExactRelationshipStatus = Config.Bind<bool>("Cheaty", "Show Exact Relationship Status", false, "If enabled, Relationship status will be shown as exact values on the slider. (Default is false)");
            Config_ShowGiftEffects = Config.Bind<bool>("Cheaty", "Show Gift Effects", false, "If enabled, Gift Effects will be shown. (Default is false)");
            Config_SortGifts = Config.Bind<bool>("Cheaty", "Sort Gifts", false, "If enabled, Gifts in the Inventory Window will be sorted by their effectiveness. (Default is false)");
        }

        public static QoLMod GetInstance()
        {
            if (!Chainloader.PluginInfos.ContainsKey("plugins.modapi"))
            {
                Debug.LogError("[QoL] Tried to obtain QoL Instance but it was not loaded!");
                return null;
            }

            return Chainloader.PluginInfos["plugins.quadruplicationoflikeability"].Instance as QoLMod;
        }

        public float GetBattleAnimationSpeed()
        {
            return Config_BattleAnimationSpeed.Value;
        }

        public float GetBattleMovementSpeed()
        {
            return Config_BattleMovementSpeed.Value;
        }

        public bool GetSmoothBattlesEnabled()
        {
            return Config_SmoothBattles.Value;
        }

        public bool GetShowExactRelationshipStatusEnabled()
        {
            return Config_ShowExactRelationshipStatus.Value;
        }

        public bool GetShowGiftEffectsEnabled()
        {
            return Config_ShowGiftEffects.Value;
        }

        public bool GetSortGiftsEnabled()
        {
            return Config_SortGifts.Value;
        }

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