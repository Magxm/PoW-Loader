using HarmonyLib;

using Heluo.Actor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace QoL.Hooks
{
    [HarmonyPatch]
    public class ActorController_SetAnimator_Hook
    {
        public static MethodBase TargetMethod()
        {
            return typeof(ActorController).GetProperty("Animator").GetGetMethod();
        }

        public static void Postfix(ref ActorController __instance, ref Animator ___animator)
        {
            //Check if we are inside of a battle actor
            ___animator.speed = 2;
        }
    }
}