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

        //Redirect Dictionary
        private StringComparer IgnoreCaseStringComparer = StringComparer.OrdinalIgnoreCase;
        public Dictionary<string, string> PathRedirections;
        public Encoding PathEncoding = Encoding.UTF8;

        public bool ExistsRedirect(string path)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            path = path.Replace("\\", "/");
            return PathRedirections.ContainsKey(path);
        }

        public string GetRedirect(string path)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

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

        //Asset Managment/Loading/Caching
        private Dictionary<string, object> assetCache;

        private Texture2D LoadTexture2D(string path)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            if (assetCache.ContainsKey(path))
            {
                return assetCache[path] as Texture2D;
            }

            if (!ExistsRedirect(path))
            {
                return default;
            }

            string rootRedirect = GetRedirect(path);
            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;

            byte[] data = File.ReadAllBytes(absolutePath);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(data);

            assetCache[path] = texture2D;

            return texture2D;
        }

        private Sprite LoadSprite(string path)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            if (assetCache.ContainsKey(path))
            {
                return assetCache[path] as Sprite;
            }

            if (!ExistsRedirect(path))
            {
                return default;
            }


            string rootRedirect = GetRedirect(path);
            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;

            byte[] data = File.ReadAllBytes(absolutePath);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(data);

            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2((float)(texture2D.width / 2), (float)(texture2D.height / 2)));

            assetCache[path] = sprite;
            return sprite;
        }

        private TextAsset LoadText(string path)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            if (assetCache.ContainsKey(path))
            {
                return assetCache[path] as TextAsset;
            }

            if (!ExistsRedirect(path))
            {
                return default;
            }


            string rootRedirect = GetRedirect(path);
            string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;

            byte[] data = File.ReadAllBytes(absolutePath);
            TextAsset ta = new TextAsset(Encoding.UTF8.GetString(data, 0, data.Length));

            assetCache[path] = ta;
            return ta;
        }

        //XUnity.ResourceRedirector stuff
        public void AssetLoading(AssetLoadingContext context)
        {
            string path = context.Parameters.Name;
            if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
            {
                path = path.Substring(7);
            }

            //Debug.LogError("AssetLoading: " + context.Parameters.Name);
            string rootRedirect = GetRedirect(path);
            if (string.IsNullOrEmpty(rootRedirect))
            {
                Debug.Log("[ResourceRedirectManager] Found no redirect for " + path);
                context.Complete(false, false, false);
                return;
            }

            if (context.Parameters.Type == typeof(Texture2D))
            {
                context.Asset = LoadTexture2D(path);
            }
            else if (context.Parameters.Type == typeof(Sprite))
            {
                context.Asset = LoadSprite(path);
            }
            else if (context.Parameters.Type == typeof(TextAsset))
            {
                context.Asset = LoadText(path);
            }

            context.Complete(true, true, true);
        }

        public Dictionary<string, string> assetNameToPath;
        public void AssetLoaded(AssetLoadedContext context)
        {
            //Debug.LogError("AssetLoaded: " + context.Parameters.Name);
            if (Path.GetExtension(context.Parameters.Name) == ".prefab")
            {
                //It is a prefab, we want to check for child GameObjects and replace them if needed
                GameObject prefab = context.Asset as GameObject;
                Component[] allChildren = prefab.GetComponentsInChildren(typeof(Component));
                //Debug.Log("Prefab: " + context.Parameters.Name);
                foreach (Component child in allChildren)
                {
                    if (child == null) continue;

                    //Checking if we can replace the content of the component based on the type of the component
                    Type childType = child.GetType();
                    if (childType == typeof(UnityEngine.UI.Image))
                    {
                        //It is an Image, checking if we can replace the image.
                        UnityEngine.UI.Image childImage = child as UnityEngine.UI.Image;
                        if (childImage.sprite != null)
                        {
                            string originalSpriteName = childImage.sprite.name;
                            string imageFileName = originalSpriteName + ".png";
                            if (assetNameToPath.ContainsKey(imageFileName))
                            {
                                string path = assetNameToPath[imageFileName];
                                if (ExistsRedirect(path))
                                {
                                    //We have a replacement for the sprite
                                    Debug.Log("Overriding the texture of the over Prefab loaded Sprite  " + childImage.sprite.name + " with the texture " + path);
                                    //Overwritting Sprite Texture
                                    string rootRedirect = GetRedirect(path);
                                    string absolutePath = Paths.GameRootPath + Path.DirectorySeparatorChar + rootRedirect + Path.DirectorySeparatorChar + path;
                                    childImage.sprite.texture.LoadImage(File.ReadAllBytes(absolutePath), false);
                                }
                                else
                                {
                                    //Debug.Log("Could not find prefab redirect for image from path " + path);
                                }
                            }
                            else
                            {
                                //Debug.Log("Could not find prefab redirect for image file " + imageFileName + " from bundle " + context.Bundle.name);
                            }
                        }
                    }
                }
            }

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
            //Caching filenames to their actual path. We need this to modify prefab game objects later on.
            foreach (string assetName in context.Bundle.GetAllAssetNames())
            {
                if (assetNameToPath.ContainsKey(assetName))
                {
                    Debug.LogError("[Resource Redirector] An Asset with name " + assetName + ". This is a big issue since we need unique asset names!");
                }
                else
                {
                    string path = assetName;
                    if (path.StartsWith("Assets/") || path.StartsWith("assets/"))
                    {
                        path = path.Substring(7);
                    }

                    string filename = Path.GetFileName(path);
                    //Debug.LogError("Remembering path for " + filename + " is " + path);
                    assetNameToPath[filename] = path;
                }
            }
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
            assetCache = new Dictionary<string, object>(IgnoreCaseStringComparer);
            assetNameToPath = new Dictionary<string, string>(IgnoreCaseStringComparer);

            //ResourceRedirection.LogAllLoadedResources = true;

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

            Debug.Log("[ResourceRedirectManager] Adding redirect for file " + path + " over root folder " + rootPath);
            if (ExistsRedirect(path))
            {
                UnityEngine.Debug.LogWarning("[ResourceRedirectManager] Mod Collision: Overwritting Resource Redirect " + path + " from " + GetRedirect(path) + " to " + rootPath);
            }
            PathRedirections[path] = rootPath;
        }
    }

}
