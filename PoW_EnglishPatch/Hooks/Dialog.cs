using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using Heluo.Data;
using Heluo.Platform;
using Heluo.UI;

using UnityEngine;
using UnityEngine.UI;

namespace PoW_EnglishPatch.Hooks
{
    /*
    [HarmonyPatch(typeof(CtrlTalk), "SetMessageView")]
    public class CtrlTalk_SetMessageView_Hook
    {
        public static int ShouldNewLineAt(string message)
        {
            Debug.Log("1.5");
            return Math.Min(message.Length, 5);
        }

        //We hook the CtrlTalk SetMessageView function and dynamically split the talk object if the message is too big.
        public static void Prefix(ref CtrlTalk __instance, ref Talk talk)
        {
            Debug.Log("1");
            //The game has filled out the satisfied list with the Talk entries that are the current queue. We iterate over it.
            List<string> lines = new List<string>();
            string message = talk.Message;
            while (true)
            {
                int lineBreak = ShouldNewLineAt(message);
                string line = message.Substring(0, lineBreak);
                message = message.Substring(lineBreak + 1, message.Length - lineBreak - 1);
                lineBreak = ShouldNewLineAt(message);
                lines.Add(line);

                if (message.Length == 0)
                {
                    break;
                }
            }
            Debug.Log("2");
            Talk last = talk;
            Talk prev = null;

            int linesProcessed = 0;
            while (linesProcessed < lines.Count)
            {
                Debug.Log("2.1");
                string newMessage = "";
                for (int i = 0; linesProcessed < lines.Count && i < 3; linesProcessed++, i++)
                {
                    newMessage += lines[linesProcessed] + "<br>";
                }

                Debug.Log("2.2 = " + newMessage);
                if (linesProcessed == lines.Count)
                {
                    //Is last
                    Debug.Log("LAST: " + newMessage);
                    last.Message = newMessage;
                }
                else
                {
                    Talk newTalk = new Talk();
                    newTalk.EmotionType = prev.EmotionType;
                    newTalk.MessageType = MessageType.Dialog;
                    newTalk.Message = newMessage;
                    newTalk.NextTalkType = TalkType.Message;
                    newTalk.NextTalk = new List<Talk>() { last };
                    newTalk.TalkerId = last.TalkerId;
                    newTalk.Talker = last.Talker;

                    if (prev == null)
                    {
                        //This is the first entry
                        newTalk.Animation = last.Animation;
                        last.Animation = "";
                        newTalk.Condition = last.Condition;
                        last.Condition = new Heluo.Flow.BaseFlowGraph();
                        newTalk.Behaviour = last.Behaviour;
                        last.Behaviour = new Heluo.Flow.BaseFlowGraph();
                        newTalk.FailTalkId = last.FailTalkId;
                        newTalk.FailTalk = last.FailTalk;
                        talk = newTalk;
                    }
                    else
                    {
                        //Insert into linked list
                        prev.NextTalk = new List<Talk>() { newTalk };
                    }

                    Debug.Log("NEW: " + newMessage);
                    prev = newTalk;
                }
            }
            Debug.Log("Calling original");
        }
    }*/
}