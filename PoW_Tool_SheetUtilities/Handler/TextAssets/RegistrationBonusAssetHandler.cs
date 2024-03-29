﻿using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class RegistrationBonusAssetHandler : AssetHandler
    {
        public RegistrationBonusAssetHandler()
        {
            SheetId = "1PZQd4Pvq4v47NvGjM7jjaeH-RIVQPgmoMPZK5yuh_Vg";
            AssetName = "RegistrationBonus";
            SheetRange = "A2:F";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "RegistrationBonus";
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
                    Name = "Desc",
                    VariableType = AssetVariableType.MachineTL
                },
                new AssetVariableDefinition()
                {
                    Name = "FourAttributesPoint",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "TraitPoint",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "UnLockTraits",
                    VariableType = AssetVariableType.NoTranslate
                },
            };
        }
    }
}