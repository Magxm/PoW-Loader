using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;

using Heluo.Actor;
using Heluo.Animation;
using Heluo.Battle;
using Heluo.Events;
using UnityEngine;

namespace QoL.Hooks
{
    //Firstly we override the Animator speed of every new UnitActorController
    [HarmonyPatch(typeof(UnitActorController), "Awake")]
    public class UnitActorController_Awake_Hook
    {
        public static bool Prefix(ref ActorController __instance)
        {
            __instance.Speed = QoLMod.GetInstance().GetBattleAnimationSpeed();
            return true;
        }

        public static void Postfix(ref ActorController __instance, ref Animator ___animator)
        {
            ___animator.speed = QoLMod.GetInstance().GetBattleAnimationSpeed();
        }
    }

    //Hooks to handle Unit Skill Movement speed
    [HarmonyPatch(typeof(WuxiaUnit), "SkillMovement")]
    public class WuxiaUnit_SkillMovement_Hook
    {
        public static void Prefix(ref WuxiaCell endCell, ref float moveTime)
        {
            moveTime /= QoLMod.GetInstance().GetBattleMovementSpeed();
        }
    }

    //Hooks to handle Unit Movement speed
    [HarmonyPatch(typeof(WuxiaBattleUnit), "create_unit")]
    public class WuxiaUnit_Ctor_Hook
    {
        public static void Postfix(ref WuxiaUnit __result)
        {
            __result.MovementSpeed *= QoLMod.GetInstance().GetBattleMovementSpeed();
        }
    }


    //BattleProcessStrategy has a ProcessAnimation function that returns the time of the clip, rather than the time the animator will need.
    //We hook it and recalculate the clip time.
    [HarmonyPatch(typeof(BattleProcessStrategy), "ProcessAnimation")]
    public class BattleProcessStrategy_ProcessAnimation_Hook
    {
        public static bool Prefix(ref Task<float> __result, ref DamageInfo damageInfo, ref float attackAnimationLength)
        {
            string effect = damageInfo.Skill.Item.Effect;
            AnimationClip clip = damageInfo.Attacker.Actor.PlayAnimation(effect, null, WrapMode.Once);
            float originalLength = (clip != null) ? clip.length : 1f;
            float resultLength = originalLength / QoLMod.GetInstance().GetBattleAnimationSpeed();

            if (QoLMod.GetInstance().GetSmoothBattlesEnabled())
            {
                //To make transitions smooth, we modify the wait time to be shorter than the actual animation
                resultLength /= 1.5f;
            }
            __result = Task.FromResult(resultLength);
            return false;
        }
    }

}