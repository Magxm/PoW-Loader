using HarmonyLib;

using Heluo.Platform;
using Heluo.UI;

using UnityEngine;
using UnityEngine.UI;

namespace EnglishPatch.Hooks
{

    [HarmonyPatch(typeof(WGText), "set_Text")]
    public class WGText_setText_Hook
    {


        public static void Postfix(ref WGText __instance, ref Text ___text)
        {
            ___text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            ___text.fontSize = 20;
            if (___text.text.Length <= 20)
            {
                ___text.horizontalOverflow = HorizontalWrapMode.Overflow;
            }
            else
            {
                ___text.horizontalOverflow = HorizontalWrapMode.Wrap;
                ___text.verticalOverflow = VerticalWrapMode.Overflow;
            }
        }
    }


    [HarmonyPatch(typeof(WGStringTable), "UpdateString")]
    public class WGStringTable_UpdateString_Hook
    {


        public static void Postfix(ref WGStringTable __instance)
        {
            __instance.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (__instance.GetComponent<Text>().text.Length <= 20)
            {
                __instance.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            }
            else
            {
                __instance.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                __instance.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            }
        }
    }


    [HarmonyPatch(typeof(UIBattleStatus), "UpdateView")]
    public class UIBattleStatus_UpdateView_Hook
    {


        public static void Postfix(ref UIBattleStatus __instance)
        {
            __instance.FinalCounterAttack.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            __instance.FinalCrit.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            __instance.FinalDodge.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            __instance.FinalHit.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            __instance.FinalParry.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        }
    }


    [HarmonyPatch(typeof(UIQuest), "SetTrackedQuest")]
    public class UIQuest_SetTrackedQuest_Hook
    {


        public static void Postfix(ref UIQuest __instance, ref WGText ___questName, ref WGText ___description)
        {
            ___questName.GetComponent<Text>().fontSize = 18;
            ___description.GetComponent<Text>().fontSize = 16;
        }
    }


    [HarmonyPatch(typeof(WGBattleInfo), "UpdateInfo")]
    public class WGBattleInfo_UpdateInfo_Hook
    {


        public static void Postfix(ref WGBattleInfo __instance, ref WGText ___Name)
        {
            ___Name.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            ___Name.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        }
    }


    [HarmonyPatch(typeof(WGPaoMaDengText), "AddMessage")]
    public class WGPaoMaDengText_AddMessage_Hook
    {


        public static void Postfix(ref WGPaoMaDengText __instance, ref WGText ___pmdText)
        {
            ___pmdText.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }


    [HarmonyPatch(typeof(WGNurturanceBtn), "UpdateButton")]
    public class WGNurturanceBtn_UpdateButton_Hook
    {


        public static void Postfix(ref WGNurturanceBtn __instance, ref WGText ___Btn_Name)
        {
            __instance.Btn_Name.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            __instance.Btn_Name.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
            __instance.Btn_Name.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
        }
    }

    [HarmonyPatch(typeof(UIRegistration), "NameCheck")]
    public class UIRegistration_NameCheck_Hook
    {


        public static bool Prefix()
        {
            return false;
        }
    }


    [HarmonyPatch(typeof(SteamPlatform), "CheckWordLegality")]
    public class SteamPlatform_CheckWordLegality_Hook
    {


        public static bool Prefix(string word, ref bool __result)
        {
            __result = true;
            return false;
        }
    }


    [HarmonyPatch(typeof(WGBookBtn), "UpdateWidget")]
    public class WGBookBtn_UpdateWidget_Hook
    {
        public static void Postfix(ref WGText ___itemName)
        {
            if (___itemName.GetComponent<Text>().text.Length <= 20)
            {
                ___itemName.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            }
            else
            {
                ___itemName.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                ___itemName.GetComponent<Text>().verticalOverflow = VerticalWrapMode.Overflow;
            }
        }
    }

    [HarmonyPatch(typeof(WGTraitBtn), "UpdateWidget")]
    public class WGTraitBtn_UpdateWidget_Hook
    {


        public static void Postfix(ref WGText ___itemName)
        {
            ___itemName.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            ___itemName.GetComponent<Text>().rectTransform.offsetMin = new Vector2(-120f, -20f);
        }
    }


    [HarmonyPatch(typeof(WGTitleValue), "TitleFormat")]
    public class WGTitleValue_TitleFormat_Hook
    {


        public static void Postfix(ref Text ___Title)
        {
            ___Title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            ___Title.fontSize = 24;
        }
    }


    [HarmonyPatch(typeof(WGTitleValue), "ValueFromat")]
    public class WGTitleValue_ValueFromat_Hook
    {


        public static void Postfix(ref Text ___Value)
        {
            ___Value.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            ___Value.fontSize = 20;
        }
    }



    [HarmonyPatch(typeof(UIRelationship), "UpdateRelationship")]
    public class UIRelationship_UpdateRelationship_Hook
    {


        public static void Postfix(ref Text ___community_name, ref Text ___community_introduction)
        {
            ___community_name.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            ___community_introduction.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }

}
