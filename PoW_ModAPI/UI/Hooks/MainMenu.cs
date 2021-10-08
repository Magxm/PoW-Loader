using HarmonyLib;

using Heluo.FSM.Main;

namespace PoW_ModAPI.UI.Hooks
{
    public class MainMenuWrapper
    {
        //Singleton Stuff
        private static MainMenuWrapper __instance = null;

        public static MainMenuWrapper GetInstance()
        {
            if (__instance == null)
            {
                __instance = new MainMenuWrapper();
            }

            return __instance;
        }

        private MainMenuWrapper()
        {
        }

        //Members
        public MainMenu GameObject;
    }

    [HarmonyPatch(typeof(MainMenu), "OpenMain")]
    public class MainMenu_OpenMain_Hook
    {
        public static bool Prefix(ref MainMenu __instance)
        {
            MainMenuWrapper.GetInstance().GameObject = __instance;
            return true;
        }
    }
}