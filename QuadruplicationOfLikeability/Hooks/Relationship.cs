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
        [HarmonyPatch(typeof(UIRelationship), "UpdateRelationship")]
        public class UI_Relationship_UpdateRelationship_Hook
        {
            //I yoinked the position from Binarizers version at https://github.com/Binarizer/Plugin-Pow/blob/master/Pow_Plugin_Binarizer/Hooks/HookFeaturesAndFixes.cs
            public static void Postfix(ref UIRelationship __instance, ref RelationshipInfo _info, ref Slider ___expbar, ref string ___currentId)
            {
                if (QoLMod.GetInstance().GetShowExactRelationshipChangeEnabled())
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
    }
}
