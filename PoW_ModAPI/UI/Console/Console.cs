using UnityEngine;

namespace PoW_ModAPI.UI.Console
{
    class Console
    {
        private static GameObject _console;
        public static void Init()
        {
            _console = new GameObject();
            _console.AddComponent<ConsoleScript>();
            GameObject.DontDestroyOnLoad(_console);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(_console);
        }
    }
}
