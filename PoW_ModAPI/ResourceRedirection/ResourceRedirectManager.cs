using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BepInEx;

using UnityEngine;

using XUnity.ResourceRedirector;

namespace ModAPI
{
    /*
     * Actual ResourceRedirectManager that handles managing the different redirect paths.
     * We have hooked the ExternalResourceProvider, however if something still goes throuhg, we use XUnity.ResourceRedirector to catch them directly in the asset loading pipeline.
    */
    public class ResourceRedirectManager
    {
        //XUnity.ResourceRedirector stuff
        public void AssetLoading(AssetLoadingContext context)
        {
            string path = context.Parameters.Name;
            if (path.StartsWith("Assets/"))
            {
                path = path.Substring(7);
            }

            //Debug.LogError("AssetLoading: " + context.Parameters.Name);
            string rootRedirect = GetRedirect(path);
            if (string.IsNullOrEmpty(rootRedirect))
            {
                Debug.LogError("Found no redirect for " + path);
                context.Complete(false, false, false);
                return;
            }

            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;

            byte[] data = File.ReadAllBytes(absolutePath);
            if (context.Parameters.Type == typeof(Texture2D) || context.Parameters.Type == typeof(Sprite))
            {

                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(data);
                if (context.Parameters.Type == typeof(Sprite))
                {
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2((float)(texture2D.width / 2), (float)(texture2D.height / 2)));
                    context.Asset = sprite;
                }
                else
                {
                    context.Asset = texture2D;
                }
            }
            else if (context.Parameters.Type == typeof(TextAsset))
            {
                TextAsset ta = new TextAsset(Encoding.UTF8.GetString(data, 0, data.Length));
                context.Asset = ta;
            }
            context.Complete(true, true, true);
        }

        public void AssetLoaded(AssetLoadedContext context)
        {
            //Debug.LogError("AssetLoaded: " + context.Parameters.Name);
            context.Complete(false);
        }

        public void AsyncAssetLoading(AsyncAssetLoadingContext context)
        {
            //Debug.LogError("AsyncAssetLoading: " + context.Parameters.Name);
            context.Complete(false, false, false);
        }

        public void AssetBundleLoading(AssetBundleLoadingContext context)
        {
            //Debug.LogError("AssetBundleLoading: " + context.Parameters.Path);
            context.Complete(false, false, false);
        }

        public void AssetBundleLoaded(AssetBundleLoadedContext context)
        {
            //Debug.LogError("AssetBundleLoaded: " + context.Parameters.Path);
            context.Complete(false);
        }

        public void AsyncAssetBundleLoading(AsyncAssetBundleLoadingContext context)
        {
            //Debug.LogError("AsyncAssetBundleLoading: " + context.Parameters.Path);
            context.Complete(false, false, false);
        }

        public void ResourceLoaded(ResourceLoadedContext context)
        {
            //Debug.LogError("ResourceLoaded: " + context.Parameters.Path);
            context.Complete(false);
        }


        //Constructor
        private ResourceRedirectManager()
        {
            PathRedirections = new Dictionary<string, string>(IgnoreCaseStringComparer);
            ResourceRedirection.LogAllLoadedResources = true;
            ResourceRedirection.EnableRedirectMissingAssetBundlesToEmptyAssetBundle(0);
            ResourceRedirection.DisableRecursionPermanently();
            ResourceRedirection.EnableRedirectMissingAssetBundlesToEmptyAssetBundle(0);

            ResourceRedirection.RegisterAssetLoadedHook(HookBehaviour.OneCallbackPerResourceLoaded, 0, AssetLoaded);
            ResourceRedirection.RegisterAssetLoadingHook(0, AssetLoading);
            ResourceRedirection.RegisterAsyncAssetLoadingHook(0, AsyncAssetLoading);

            ResourceRedirection.RegisterAssetBundleLoadingHook(0, AssetBundleLoading);
            ResourceRedirection.RegisterAssetBundleLoadedHook(0, AssetBundleLoaded);
            ResourceRedirection.RegisterAsyncAssetBundleLoadingHook(0, AsyncAssetBundleLoading);


            ResourceRedirection.RegisterResourceLoadedHook(HookBehaviour.OneCallbackPerResourceLoaded, 0, ResourceLoaded);
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
