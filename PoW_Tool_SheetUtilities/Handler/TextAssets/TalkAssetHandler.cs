using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class TalkAssetHandler : AssetHandler
    {
        public TalkAssetHandler()
        {
            SheetId = "1lmIjyewgIZfy2tqanCz1h1aPaKDcsbMOKOu1W5sjQzw";
            AssetName = "Talk";
            SheetRange = "A2:O";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Talk";
            OutputExtension = ".txt";

            VariableDefinitions = new List<AssetVariableDefinition>()
            {
                new AssetVariableDefinition()
                {
                    Name = "ID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "TalkerID",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "EmotionType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "MessageType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Message",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "FailTalkId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "NextTalkType",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "NextTalkId",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Animation",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Condition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Behaviour",
                    VariableType = AssetVariableType.Translate,
                    AutoML = false,
                    IsScriptField_FilterNoText = true,
                },
            };
        }
    }
}