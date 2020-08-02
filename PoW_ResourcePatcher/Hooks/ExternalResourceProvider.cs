using System.Reflection;

using HarmonyLib;

using Heluo.Resource;

using UnityEngine;

namespace PoW_EnglishPatch.Hooks
{
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

    [HarmonyPatch]
    public class ExternalResourceProvider_LoadBytes_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("LoadBytes");
        }

        public static void Prefix(ExternalResourceProvider __instance, ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ExternalResourceProvider_Hooks.ModifiedExternalDirectory;
        }
    }


    [HarmonyPatch]
    public class ExternalResourceProvider_Load_Sprite_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(Sprite));
        }

        public static void Prefix(ExternalResourceProvider __instance, ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ExternalResourceProvider_Hooks.ModifiedExternalDirectory;
        }
    }


    [HarmonyPatch]
    public class ExternalResourceProvider_Load_AudioClip_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(AudioClip));
        }

        public static void Prefix(ExternalResourceProvider __instance, ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ExternalResourceProvider_Hooks.ModifiedExternalDirectory;
        }
    }

    [HarmonyPatch]
    public class ExternalResourceProvider_Load_GameObject_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(GameObject));
        }

        public static void Prefix(ExternalResourceProvider __instance, ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ExternalResourceProvider_Hooks.ModifiedExternalDirectory;
        }
    }

    [HarmonyPatch]
    public class ExternalResourceProvider_Load_ShaderVariantCollection_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ExternalResourceProvider).GetMethod("Load").MakeGenericMethod(typeof(ShaderVariantCollection));
        }

        public static void Prefix(ExternalResourceProvider __instance, ref string ___ExternalDirectory, ref string path)
        {
            ___ExternalDirectory = ExternalResourceProvider_Hooks.ModifiedExternalDirectory;
        }
    }

    public class ExternalResourceProvider_Hooks
    {
        public static string ModifiedExternalDirectory = "Mods/EnglishTranslate/";
    }
}
