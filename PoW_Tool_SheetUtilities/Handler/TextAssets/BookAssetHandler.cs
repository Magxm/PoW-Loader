using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class BookAssetHandler : AssetHandler
    {
        public BookAssetHandler()
        {
            SheetId = "1lscQ2-OOC-HXOc02G6ACTECObtVflCXUCMpsFe3CUac";
            AssetName = "Book";
            SheetRange = "A2:AB";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Book";
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
                    Name = "Name",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "isLibary",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "BookTab",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "IconName",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "MaxReadTime",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "ReadEffect",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "LearnSkill",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "ReadCondition",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "ReadConditionDescription",
                    VariableType = AssetVariableType.Translate,
                },
                new AssetVariableDefinition()
                {
                    Name = "ShowCondition",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "TipsCanRead",
                    VariableType = AssetVariableType.Translate,
                },
                new AssetVariableDefinition()
                {
                    Name = "TipsCanNotRead",
                    VariableType = AssetVariableType.Translate,
                },
                new AssetVariableDefinition()
                {
                    Name = "NotReadFinishMovie",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "ReadFinishMovie",
                    VariableType = AssetVariableType.NoTranslate,
                },
            };
        }
    }
}