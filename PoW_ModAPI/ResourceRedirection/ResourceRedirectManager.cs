using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using BepInEx;

using HarmonyLib;

using Heluo;
using Heluo.Resource;

using UnityEngine;

using XUnity.ResourceRedirector;

namespace ModAPI
{

    /*
    [HarmonyPatch(typeof(string), "IsNullOrEmpty", new Type[] { typeof(string) })]
    public class String_IsNullOrEmpty_Hook
    {
        public static void Prefix(ref string value)
        {
            var stackFrame = new StackFrame(2, false);
            var methodBase = stackFrame.GetMethod();
            if (methodBase.DeclaringType == typeof(ExternalResourceProvider) && methodBase.Name.Equals("Load"))
            {
                var mi = methodBase as MethodInfo;
                var t = mi.ReturnType;
                UnityEngine.Debug.LogWarning(mi.FullDescription() + " from " + (new StackFrame(3, false).GetMethod() as MethodInfo).FullDescription());
            }
        }
    }
    */

    //Hooks of LoadBytes and Load<T> that redirect the loading if needed. This is the games own ExternalResourceProvider and will work for thes language files and some assets.
    //This will not work for all bundles tho.
    [HarmonyPatch(typeof(ExternalResourceProvider), "LoadBytes", new Type[] { typeof(string) })]
    public class ExternalResourceProvider_LoadBytes_Hook
    {
        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            UnityEngine.Debug.Log("Loading Raw " + path);
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }

    [HarmonyPatch]
    public class ResourceManagerr_Load_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ResourceManager).GetMethod("Reset");
        }

        public static void Postfix(ref IChainedResourceProvider ___provider)
        {
            //Flipping arround the first provider with it's successor. This way ExternalResourcePRovider will be before BuiltinResourceProvider
            IChainedResourceProvider builtinProvider = ___provider;
            IChainedResourceProvider externalProvider = builtinProvider.Successor;
            IChainedResourceProvider thirdProvider = externalProvider.Successor;

            ___provider = externalProvider;
            externalProvider.Successor = builtinProvider;
            builtinProvider.Successor = thirdProvider;
        }

    }

    [HarmonyPatch]
    public class ExternalResourceProvider_Load_Hook
    {

        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(UnityEngine.Object));
        }

        //This hook breaks the Load<T> function. All of them are Load<UnityEngine.Object> now.
        //So we gotta do all the loading ourself and return the proper thing.
        public static bool Prefix(ref string path, ref UnityEngine.Object __result)
        {
            UnityEngine.Debug.Log("[ResourceRedirectManager] Loading Request: " + path);
            string rootRedirect = ResourceRedirectManager.GetInstance().GetRedirect(path);
            if (string.IsNullOrEmpty(rootRedirect))
            {
                //We do not have a redirect. Return null and do not execute original function
                UnityEngine.Debug.Log("No redirect found for " + path);
                __result = null;
                return false;
            }

            bool flag = path.Contains("Config") && path.Contains(GameConfig.Language);
            if (flag)
            {
                path.Replace("/" + GameConfig.Language, "");
            }

            UnityEngine.Debug.Log("[ResourceRedirectManager] Loading " + path);

            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;

            if (!File.Exists(absolutePath))
            {
                UnityEngine.Debug.LogError("[ResourceRedirectManager] Error while loading " + absolutePath + ". File does not exist!");
                __result = null;
                return false;
            }

            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".png":
                    //Parse Sprite
                    byte[] data = File.ReadAllBytes(absolutePath);
                    Texture2D texture2D = new Texture2D(2, 2);
                    texture2D.LoadImage(data);
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2((float)(texture2D.width / 2), (float)(texture2D.height / 2)));
                    __result = sprite;
                    break;
                case ".prefab":
                    //Parse GameObject
                    break;
                default:
                    UnityEngine.Debug.LogError("[ResourceRedirectManager] Error while loading " + path + " with redirect " + rootRedirect + ". Unknown Extension " + extension);
                    __result = null;
                    break;
            }

            return false;
        }
    }


    /*
     * Actual ResourceRedirectManager that handles managing the different redirect paths.
     * We have hooked the ExternalResourceProvider, however if something still goes throuhg, we use XUnity.ResourceRedirector to catch them directly in the asset loading pipeline.
    */
    public class ResourceRedirectManager
    {
        //Constructor
        private ResourceRedirectManager()
        {
            PathRedirections = new Dictionary<string, string>(IgnoreCaseStringComparer);
        }

        //XUnity.ResourceRedirector stuff
        public void AssetLoaded(AssetLoadedContext context)
        {

        }

        //Singleton stuff
        private static ResourceRedirectManager instance = null;
        public static ResourceRedirectManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ResourceRedirectManager();
            }

            return instance;
        }

        //Redirect Dictionary
        private StringComparer IgnoreCaseStringComparer = StringComparer.OrdinalIgnoreCase;
        public Dictionary<string, string> PathRedirections;
        public Encoding PathEncoding = Encoding.UTF8;

        public bool ExistsRedirect(string path)
        {
            path = path.Replace("\\", "/");
            return PathRedirections.ContainsKey(path);
        }

        public string GetRedirect(string path)
        {
            path = path.Replace("\\", "/");

            string result;
            PathRedirections.TryGetValue(path, out result);
            if (!string.IsNullOrEmpty(result))
            {
                //UnityEngine.Debug.Log(path + " redirects to RootPath " + result);
                return result;
            }
            else
            {
                return string.Empty;
            }

        }



        /// <summary>
        /// Adds all files including files in subfolders in a directory to the redirect list.
        /// Paths from the given directory must be equivilant to the paths inside the game files.
        /// If the same file exists more than once the LAST registered one will be remembered.
        /// If the game tries to read a file that has a redirection setup, we will redirect the load to our own file.
        /// Example: If we registered "Mods\Translate" and "Config\chs\Battle\Buffer\a.json" is a valid path inside the Translate folder
        /// then if the game tries to load "Config\chs\Battle\Buffer\a.json" from the game files it will be redirected
        /// to "GamePath\Mods\Translate\Config\chs\Battle\Buffer\a.json".
        /// </summary>
        /// <param name="path">Relative path to game root folder</param>
        public void AddRessourceFolder(string path)
        {
            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + path;
            if (Directory.Exists(absolutePath))
            {
                // This path is a valid directory
                UnityEngine.Debug.Log("[ResourceRedirectManager] Adding Ressource Folder " + absolutePath + " as " + path);
                AddDirectoryToPathRedirection(path, "");
            }
            else
            {
                UnityEngine.Debug.LogError("[ResourceRedirectManager] Error while adding ressource folder: " + absolutePath + " is not a valid directory!");
            }
        }

        private void AddDirectoryToPathRedirection(string rootPath, string directoryPath)
        {
            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootPath + Path.DirectorySeparatorChar + directoryPath;
            if (Directory.Exists(absolutePath))
            {
                DirectoryInfo dInfo = new DirectoryInfo(absolutePath);
                var subdirectories = dInfo.EnumerateDirectories();
                foreach (var dir in subdirectories)
                {
                    string newDirectoryPath = directoryPath;
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        newDirectoryPath += Path.DirectorySeparatorChar;
                    }
                    newDirectoryPath += dir.Name;
                    AddDirectoryToPathRedirection(rootPath, newDirectoryPath);
                }

                var files = dInfo.EnumerateFiles();
                foreach (var file in files)
                {
                    AddFileToPathRedirection(rootPath, directoryPath + Path.DirectorySeparatorChar + file.Name);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("[ResourceRedirectManager] Internal Error while adding ressource folder: " + absolutePath + " is not a valid directory!");
            }
        }

        private void AddFileToPathRedirection(string rootPath, string path)
        {
            //Game uses / instead of the Path.DirectorySeparatorChar which is \
            path = path.Replace("\\", "/");
            rootPath = rootPath.Replace("\\", "/");

            //Debug.Log("[ResourceRedirectManager] Adding redirect for file " + path + " over root folder " + rootPath);
            if (ExistsRedirect(path))
            {
                UnityEngine.Debug.LogWarning("[ResourceRedirectManager] Mod Collision: Overwritting Resource Redirect " + path + " from " + GetRedirect(path) + " to " + rootPath);
            }
            PathRedirections[path] = rootPath;
        }
    }

}
