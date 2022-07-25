using HarmonyLib;

using Heluo.Battle;

using UnityEngine;

namespace PoW_ModAPI.DebugHooks
{
    [HarmonyPatch(typeof(WuxiaBattleSchedule), "InitBattleScheduleData")]
    public class WuxiaBattleSchedule_InitBattleScheduleData_Hook
    {
        public static bool Prefix(string ScheduleID)
        {
            Debug.LogError("Loading BattleSchedule " + ScheduleID);
            return true;
        }
    }
}