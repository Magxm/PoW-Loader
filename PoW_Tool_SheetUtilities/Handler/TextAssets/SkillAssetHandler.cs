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
                    Name = "J",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Requirement Type",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Requirement Amount",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "M",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "N",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "O",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "P",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Q",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "R",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "S",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "T",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "U",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "V",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "W",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "X",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Y",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Z",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "AA",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "AB",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "AC",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Script",
                    VariableType = AssetVariableType.NoTranslate,
                },
            };
        }
    }
}