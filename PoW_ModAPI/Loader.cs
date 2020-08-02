using PoW_ModAPI.UI.Console;

using UnityEngine;

namespace PoW_ModAPI
{
    public class Loader
    {
        //private static GameObject _inspector;
        private static GameObject _main;
        public static void Init()
        {
            Console.Init();

            _main = new GameObject();
            _main.AddComponent<Main>();
            GameObject.DontDestroyOnLoad(_main);
            /*
            _inspector = new GameObject();
            _inspector.AddComponent<InspectorScript>();
            GameObject.DontDestroyOnLoad(_inspector);
            */
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(_main);
            //GameObject.Destroy(_inspector);
        }
    }
}
