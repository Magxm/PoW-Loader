using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Heluo;
using Heluo.Battle;
using Heluo.Data;
using Heluo.Manager;
using Heluo.UI;
using QoL;
using UnityEngine;
using UnityEngine.UI;

namespace PoW_QuadruplicationOfLikeability.Hooks
{
    public class Relationship
    {

        public class Relationship_Info_Storage
        {
            private static Relationship_Info_Storage __instance;
            public static Relationship_Info_Storage GetInstance()
            {
                if (__instance == null)
                {
                    __instance = new Relationship_Info_Storage();
                }
                return __instance;
            }

            public bool Opened = false;
            public string CurrentId = "";
        }

        //Hooking UIRelationship's Show and Hide function to track if we are currently inside the UIRelationship window
        [HarmonyPatch(typeof(UIRelationship), "Show")]
        public class UIRelationship_Show_Hook
        {
            public static void Prefix()
            {
                Relationship_Info_Storage.GetInstance().Opened = true;
            }
        }

        [HarmonyPatch(typeof(UIRelationship), "Hide")]
        public class UIRelationship_Hide_Hook
        {
            public static void Prefix()
            {
                Relationship_Info_Storage.GetInstance().Opened = false;
            }
        }

        //Hooking UpdateRelationship of UIRelationship and adding a text that displays the current relationship numbers
        [HarmonyPatch(typeof(UIRelationship), "UpdateRelationship")]
        public class UIRelationship_UpdateRelationship_Hook
        {
            //I yoinked the position from Binarizers version at https://github.com/Binarizer/Plugin-Pow/blob/master/Pow_Plugin_Binarizer/Hooks/HookFeaturesAndFixes.cs
            public static void Postfix(ref UIRelationship __instance, ref RelationshipInfo _info, ref Slider ___expbar, ref string ___currentId)
            {
                Relationship_Info_Storage.GetInstance().CurrentId = ___currentId;
                if (QoLMod.GetInstance().GetShowExactRelationshipStatusEnabled())
                {
                    var text = ___expbar.GetComponentInChildren<Text>();
                    if (text == null)
                    {
                        GameObject gameObject = new GameObject("Text");
                        gameObject.transform.SetParent(___expbar.transform, false);
                        text = gameObject.AddComponent<Text>();
                        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                        text.fontSize = 25;
                        text.alignment = TextAnchor.MiddleLeft;
                        text.rectTransform.sizeDelta = new Vector2(120f, 40f);
                        text.transform.localPosition = new Vector3(-5f, 46f, 0f);
                    }

                    FavorabilityData favData = Game.GameData.Community[___currentId].Favorability;
                    text.text = $"{favData.Exp}/{favData.MaxExp}";
                }
            }
        }


        //Hooking the Name and PropsEffectDescription of the Props class to manipulate it to show the effects
        [HarmonyPatch(typeof(Props), "Name", MethodType.Getter)]
        public class Props_Name_Hook
        {
            public static void Postfix(ref Props __instance, ref string __result)
            {
                if (QoLMod.GetInstance().GetShowGiftEffectsEnabled())
                {
                    if (Relationship_Info_Storage.GetInstance().Opened && Relationship_Info_Storage.GetInstance().CurrentId != "")
                    {

                        var pFav = __instance.PropsEffect.Find(p => p is PropsFavorable pF && pF.Npcid == Relationship_Info_Storage.GetInstance().CurrentId);
                        if (pFav != null)
                        {
                            __result += $" (+{((PropsFavorable)pFav).Value})";
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Props), "PropsEffectDescription", MethodType.Getter)]
        public class Props_ProfsEffectDescriptionGetter_Hook
        {
            public static void Postfix(Props __instance, ref string __result)
            {
                if (__instance.PropsType != PropsType.Present)
                    return;

                if (!QoLMod.GetInstance().GetShowGiftEffectsEnabled())
                    return;


                var currentNpcId = Relationship_Info_Storage.GetInstance().CurrentId;
                Debug.Log("currentNpcId: " + currentNpcId);
                __result += "\n\n";
                if (Relationship_Info_Storage.GetInstance().Opened)
                {
                    //Show Effect on current opened Character
                    PropsFavorable pFav = (PropsFavorable)__instance.PropsEffect.Find(p => p is PropsFavorable pF && pF.Npcid == currentNpcId);
                    if (pFav != null)
                    {
                        __result += Game.GameData.Exterior[currentNpcId].FullName() + " " + Game.Data.Get<StringTable>("General_Favorability").Text + " +" + pFav.Value + "\n";
                        __result += "\nOthers:\n";
                    }
                }

                //Show Effect on all characters
                foreach (var pEff in __instance.PropsEffect)
                {
                    if (pEff is PropsFavorable pFav)
                    {
                        if (!Relationship_Info_Storage.GetInstance().Opened || pFav.Npcid != currentNpcId)
                        {
                            __result += Game.GameData.Exterior[pFav.Npcid].FullName() + " " + Game.Data.Get<StringTable>("General_Favorability").Text + " +" + pFav.Value + "\n";
                        }
                    }
                }
            }
        }

        //Hooking UIGiftInvertoryWindow's "Open Window" function to sort the inventory
        [HarmonyPatch(typeof(UIGiftInvertoryWindow), "OpenWindow")]
        public class UIGiftInvertoryWindow_OpenWindow_Hook
        {
            public static void Prefix(ref UIGiftInvertoryWindow __instance, ref InventoryWindowInfo inventoryWindowInfo)
            {
                if (QoLMod.GetInstance().GetSortGiftsEnabled())
                {
                    //We first get the current character id
                    var currentNpcId = Relationship_Info_Storage.GetInstance().CurrentId;

                    //We then sort the PropsInfo list called "Sort" inside the inventoryWindowInfo based on how much they effect they have on our currently selected Character
                    inventoryWindowInfo.Sort.Sort((x, y) =>
                    {

                        var xFav = x.Item.PropsEffect.Find(p => p is PropsFavorable pFav && pFav.Npcid == currentNpcId);
                        var yFav = y.Item.PropsEffect.Find(p => p is PropsFavorable pFav && pFav.Npcid == currentNpcId);

                        if (xFav == null && yFav == null)
                        {
                            return x.Item.Name.CompareTo(y.Item.Name);
                        }
                        if (xFav == null)
                            return 1;
                        if (yFav == null)
                            return -1;
                        return ((PropsFavorable)xFav).Value.CompareTo(((PropsFavorable)yFav).Value);
                    });
                }
            }
        }

    }
}
