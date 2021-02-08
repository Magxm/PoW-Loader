using System;
using System.IO;
using System.Collections.Generic;

namespace PoW_Tool_SheetUtilities.Handler.TextAssets
{
    internal class NoteDescriptionAssetHandler : AssetHandler
    {
        public NoteDescriptionAssetHandler()
        {
            SheetId = "1IiMZ_IjFSmCKZqB2eRc0u3JGXelcCQReNKhHY2GUP-4";
            AssetName = "NoteDescription";
            SheetRange = "A2:E";
            FilePathWithoutExtension = "chs" + Path.DirectorySeparatorChar + "textfiles" + Path.DirectorySeparatorChar + "NoteDescription";
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
                    Name = "ShowCondition",
                    VariableType = AssetVariableType.NoTranslate
                },
                new AssetVariableDefinition()
                {
                    Name = "Note",
                    VariableType = AssetVariableType.Translate
                },
            };
        }
    }
}