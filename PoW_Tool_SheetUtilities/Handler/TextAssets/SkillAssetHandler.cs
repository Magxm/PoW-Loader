using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using PoW_Tool_SheetUtilities.MachineTranslator;

using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class SkillAssetHandler : AssetHandler
    {
        public SkillAssetHandler()
        {
            SheetId = "1DgoiJ-o9cJhZI9mNhpi7XK38DGAkZrb__fODcUUGDRU";
            AssetName = "Skill";
            SheetRange = "A2:AD";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "Skill";
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
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "Rank",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "RequireAttribute",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "RequireValue",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "IconName",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Icon",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Type",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "DamageType",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "TargetType",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "TargetArea",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "MaxRange",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "MinRange",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "AOE",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "Algorithm",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "RequestMP",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "MaxCD",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "TargetBuffList",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "SelfBuffList",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Effect",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "PushDistance",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "Summonid",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Rewards",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "FullSkillTransformId",
                    VariableType = AssetVariableType.NoTranslate,
                }
            };
        }
    }
}