using HarmonyLib;

namespace PoW_EnglishPatch
{
    public class Loader
    {
        private static Harmony hm;
        public static void Init()
        {
            Harmony.DEBUG = true;
            hm = new Harmony("EnglishPatch");
            hm.PatchAll();
        }

        private static void Unload()
        {
            hm.UnpatchAll();
        }

        public static void Main(string[] args)
        {
            Init();
        }

    }
}
