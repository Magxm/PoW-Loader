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
using Heluo.Utility;
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
                //We check the stacktrace and make sure we only do this if the caller is not the "Process" method from the "SummonProcessStrategy" class.
                StackTrace stackTrace = new StackTrace();
                bool isSummonProcess = false;
                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    MethodBase method = frame.GetMethod();
                    if (method.DeclaringType.Name != "SummonProcessStrategy")
                    {
                        isSummonProcess = true;
                        break;
                    }
                }

                if (!isSummonProcess)
                {
                    resultLength /= 1.5f;
                }
            }

            __result = Task.FromResult(resultLength);
            return false;
        }
    }

    //We hook ActorController's "PlayCustom" to make sure the callback
    //is called at the right time rather than the original one if Smooth battle and/or animation speed is enabled
    [HarmonyPatch(typeof(ActorController), "PlayCustom", new Type[] { typeof(AnimationClip), typeof(float), typeof(Action) })]
    public class ActorController_PlayCustom_Hook
    {
        public static void Prefix(ref ActorController __instance, ref AnimationClip clip, ref float time, ref Action callback)
        {
            if (QoLMod.GetInstance().GetSmoothBattlesEnabled())
            {
                time /= 1.5f;
            }
        }
    }

    [HarmonyPatch(typeof(ActorController), "PlayCustom", new Type[] { typeof(AnimationClip), typeof(bool), typeof(Action) })]
    public class ActorController_PlayCustom_Hook2
    {
        public static bool Prefix(ref ActorController __instance, ref AnimationClip clip, ref bool callStop, ref Action callback, ref AnimatorOverrideController ___controller, ref Action ___call_back, ref Timer ___timer, ref ActorLinkPoint ___actorlinkpoint)
        {
            //We override the whole function
            ActorLinkPoint actorLinkPoint = ___actorlinkpoint;
            if (actorLinkPoint != null)
            {
                actorLinkPoint.ResetPoint();
            }
            __instance.Animator.ResetTrigger(ActorController.Params.LeaveCustom);
            __instance.Animator.ResetTrigger(ActorController.Params.Custom);
            ___controller[ActorController.State.Custom.ToString().ToLower()] = clip;
            __instance.Animator.SetTrigger(ActorController.Params.Custom);
            ___call_back = callback;
            if (callback != null)
            {
                if (___timer != null)
                {
                    ___timer.Stop();
                }
                var length = clip.length;
                if (QoLMod.GetInstance().GetSmoothBattlesEnabled())
                {
                    length /= 1.5f;
                }

                if (callStop)
                {
                    ___timer = new Timer(length, new Action(__instance.StopCustomCallBack));
                }
                else
                {
                    ___timer = new Timer(length, callback);
                }
                ___timer.Start();
            }

            //Do not call original
            return false;
        }
    }
}