using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using BepInEx;

using HarmonyLib;

using Heluo.Resource;

using PoW_ModAPI.Utils;

using UnityEngine;

namespace ModAPI
{
    //Actual ResourceRedirectManager that handles managing the different redirect paths
    public class ResourceRedirectManager
    {

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
                Debug.Log(path + " redirects to RootPath " + result);
                return result;
            }
            else
            {
                return string.Empty;
            }

        }

        //Singleton stuff
        private static ResourceRedirectManager instance = null;
        private ResourceRedirectManager()
        {
            PathRedirections = new Dictionary<string, string>(IgnoreCaseStringComparer);
        }

        public static ResourceRedirectManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ResourceRedirectManager();
            }

            return instance;
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
                Debug.Log("[ResourceRedirectManager] Adding Ressource Folder " + absolutePath + " as " + path);
                AddDirectoryToPathRedirection(path, "");
            }
            else
            {
                Debug.LogError("[ResourceRedirectManager] Error while adding ressource folder: " + absolutePath + " is not a valid directory!");
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
                Debug.LogError("[ResourceRedirectManager] Internal Error while adding ressource folder: " + absolutePath + " is not a valid directory!");
            }
        }

        private void AddFileToPathRedirection(string rootPath, string path)
        {
            //Game uses / instead of the Path.DirectorySeparatorChar which is \
            path = path.Replace("\\", "/");
            rootPath = rootPath.Replace("\\", "/");

            if (!UTF8Checker.IsUtf8(path.Select(c => (byte)c).ToArray(), path.Length))
            {
                Debug.LogError("AddFileToPathRedirection path Parameter " + path + " is not UTF8");
            }

            Debug.Log("[ResourceRedirectManager] Adding redirect for file " + path + " over root folder " + rootPath);
            if (ExistsRedirect(path))
            {
                Debug.LogWarning("[ResourceRedirectManager] Mod Collision: Overwritting Resource Redirect " + path + " from " + GetRedirect(path) + " to " + rootPath);
            }
            PathRedirections[path] = rootPath;
        }
    }

    //Actual Hooks of LoadBytes and Load<T> that redirect the loading if needed
    [HarmonyPatch(typeof(ExternalResourceProvider), "LoadBytes", new Type[] { typeof(string) })]
    public class ExternalResourceProvider_LoadBytes_Hook
    {
        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }


    [HarmonyPatch]
    public class ExternalResourceProvider_Load_Sprite_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(Sprite));
        }

        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }


    [HarmonyPatch]
    public class ExternalResourceProvider_Load_AudioClip_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(AudioClip));
        }

        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }

    [HarmonyPatch]
    public class ExternalResourceProvider_Load_GameObject_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(GameObject));
        }

        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }

    [HarmonyPatch]
    public class ExternalResourceProvider_Load_ShaderVariantCollection_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(ShaderVariantCollection));
        }

        public static void Prefix(ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ResourceRedirectManager.GetInstance().GetRedirect(path);
        }
    }

    /*
//Hooking String IsNullOrEmpty which is called from Load<T>.
//We can use this to check if there are any generic functions of Load<T> that we do not have hooked.
[HarmonyPatch(typeof(string), "IsNullOrEmpty", new Type[] { typeof(string) })]
public class String_IsNullOrEmpty_Hook
{
private static List<Type> typesHooked = new List<Type>()
{
typeof(Sprite),
typeof(AudioClip),
typeof(GameObject),
typeof(ShaderVariantCollection)
};

public static string unhookedTypeLog = "";
public static void Prefix(ref string value)
{
var stackFrame = new StackFrame(2, false);
var methodBase = stackFrame.GetMethod();
if (methodBase.DeclaringType == typeof(ExternalResourceProvider) && methodBase.Name.Equals("Load"))
{
    var mi = methodBase as MethodInfo;
    var t = mi.ReturnType;

    var normalParams = mi.GetParameters();

    if (!typesHooked.Contains(t))
    {
        UnityEngine.Debug.Log("New Unhooked Load generic function: " + methodBase.Name + " => Returns " + t.Name);
    }
}
}
}
*/
}
