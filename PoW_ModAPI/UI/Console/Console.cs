using UnityEngine;

namespace ModAPI.UI
{
    class Console
    {
        private static GameObject _Console;
        public static void Init()
        {
            _Console = new GameObject();
            _Console.AddComponent<ConsoleScript>();
            GameObject.DontDestroyOnLoad(_Console);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(_Console);
        }
    }
}
