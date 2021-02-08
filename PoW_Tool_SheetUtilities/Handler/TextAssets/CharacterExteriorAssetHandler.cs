using System;
using System.Collections.Generic;
using System.IO;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class CharacterExteriorAssetHandler : AssetHandler
    {
        public CharacterExteriorAssetHandler()
        {
            SheetId = "1MOEEq8dsJas0B2DlZy3rqtAcinjKysL6-JmTViyA01Q";
            AssetName = "CharacterExterior";
            SheetRange = "A2:Y";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "CharacterExterior";
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
                    Name = "Remark",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "SurName",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Name",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Nickname",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Protrait",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Description",
                    VariableType = AssetVariableType.Translate
                },
                new AssetVariableDefinition()
                {
                    Name = "Gender",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Size",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Model",
                    VariableType = AssetVariableType.NoTranslate,
                },
                 new AssetVariableDefinition()
                {
                    Name = "AnimMapId",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "Height",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "PreferenceType",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "IsShowProtrait",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "InfoId",
                    VariableType = AssetVariableType.NoTranslate,
                },
                new AssetVariableDefinition()
                {
                    Name = "AgeGroup",
                    VariableType = AssetVariableType.NoTranslate,
                },
            };
        }
    }
}