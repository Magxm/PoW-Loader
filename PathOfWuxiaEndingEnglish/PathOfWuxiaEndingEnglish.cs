using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using Heluo;
using Heluo.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PathOfWuxiaEndingEnglish
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInProcess("PathOfWuxia.exe")]
    public class PathOfWuxiaEndingEnglish : BaseUnityPlugin
    {
        private const string modGUID = "PathOfWuxiaEndingEnglish";
        private const string modName = "PathOfWuxiaEndingEnglish";
        private const string modVersion = "0.0.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private readonly char directorySeparatorChar = Path.DirectorySeparatorChar;

        // Developer mode enables certain commands that assist with debugging ending translations.
        // Pressing J generates a csv of all ending text based on the <EndingIds> file
        // Pressing H plays all endings. These endings can be played from anywhere, even in the title screen.
        private const bool developerMode = false;

        // There are a few ways to disable ending translations.
        // This may be useful for grabbing original text
        // Option 1: Set this variable to true.
        // Option 2: Remove the translation csv.
        private const bool blockTranslation = false;

        // a list of ending ids, taken from a file.
        // note, ending ids were retrieved manually through extracting the prefabs
        private List<string> endingIds;

        void Awake()
        {
            HarmonyFileLog.Enabled = developerMode;
        }

        void Start()
        {
            // populate endingIds
            endingIds = new List<string>();
            try
            {
                StreamReader reader = new StreamReader($"ModResources{directorySeparatorChar}EnglishModExtras{directorySeparatorChar}EndingIds.txt");
                string endingId;
                while ((endingId = reader.ReadLine()) != null)
                {
                    endingIds.Add(endingId);
                }
                reader.Close();
            }catch(Exception ex)
            {
                FileLog.Log("Exception likely due to EndingIds.txt missing. " + ex.Message);
            }

            // populate translation dictionary, where the key is the id of the text and the value is the translation
            Dictionary<string, string> endingTranslationDict = new Dictionary<string, string>();
            try
            {
                StreamReader reader = new StreamReader($"ModResources{directorySeparatorChar}EnglishModExtras{directorySeparatorChar}EndingTranslations.csv");
                string translationLine;
                bool translationFirstLine = true;
                while ((translationLine = reader.ReadLine()) != null)
                {
                    if (translationFirstLine)
                    {
                        translationFirstLine = false;
                        continue;
                    }

                    ParseCsvEndingTranslation(endingTranslationDict, translationLine);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                FileLog.Log("Exception likely due to EndingTranslations.csv missing. " + ex.Message);
            }

            // translate endings based on translation dictionary
            if (!blockTranslation)
            {
                foreach (string endingIdIter in endingIds)
                {
                    // Note, do not use <directorySeparatorChar> in Game.Resource.Load, it breaks Heluo's function
                    string pathEnding = $"UI/UIFrom/UIEnding_chs/{endingIdIter}.prefab";
                    GameObject endingObj = Game.Resource.Load<GameObject>(pathEnding);

                    if(endingObj == null)
                    {
                        continue;
                    }

                    // Location 1, endingObj.Text.Text00, single text, very commonly found as titles
                    Text titleText = endingObj.transform.Find("Text")?.Find("Text00")?.GetComponent<Text>();
                    if (titleText != null && endingTranslationDict.ContainsKey($"{endingIdIter} title"))
                    {
                        titleText.text = endingTranslationDict[$"{endingIdIter} title"];
                        FormatText(titleText, 16);
                    }

                    // Location 2, endingObj.Text.Grouop, list of text, very commonly found as paragraph text
                    Transform lineParent = endingObj.transform.Find("Text")?.Find("Grouop");
                    if (lineParent != null)
                    {
                        foreach (Transform lineTran in lineParent)
                        {
                            if (endingTranslationDict.ContainsKey($"{endingIdIter} line {lineTran.name}"))
                            {
                                Text lineTranText = lineTran.GetComponent<Text>();
                                lineTranText.text = endingTranslationDict[$"{endingIdIter} line {lineTran.name}"];
                                FormatText(lineTranText, 16);
                            }
                        }
                    }

                    // Location 3, endingObj.Text.Grouop01, list of text, used when Grouop is full, found in ending0106f_in0209dead_sub
                    Transform lineParent2 = endingObj.transform.Find("Text")?.Find("Grouop01");
                    if (lineParent2 != null)
                    {
                        foreach (Transform lineTran2 in lineParent2)
                        {
                            if (endingTranslationDict.ContainsKey($"{endingIdIter} line2 {lineTran2.name}"))
                            {
                                Text lineTran2Text = lineTran2.GetComponent<Text>();
                                lineTran2Text.text = endingTranslationDict[$"{endingIdIter} line2 {lineTran2.name}"];
                                FormatText(lineTran2Text, 16);
                            }
                        }
                    }

                    // Location 4, endingObj, list of text, found in ending0104_b
                    // note, a few lines are hard to see in white, we change the colour to black
                    foreach (Transform endingObjChild in endingObj.transform)
                    {
                        Text cinematicText = endingObjChild.GetComponent<Text>();
                        if (cinematicText != null && endingTranslationDict.ContainsKey($"{endingIdIter} cinematic {endingObjChild.name}"))
                        {
                            cinematicText.text = endingTranslationDict[$"{endingIdIter} cinematic {endingObjChild.name}"];
                            FormatText(cinematicText, 72);
                        }
                    }

                    Game.Resource.Unload(pathEnding);
                }
            }
        }

        void Update()
        {
            // plays all endings in EndingIds
            if (developerMode && Input.GetKeyDown(KeyCode.H))
            {
                // loading ui ending seems to be what's causing the black screen
                UIEnding uiEnding = Game.UI.Open<UIEnding>();

                foreach(string endingId in endingIds)
                {
                    uiEnding.AddEnding(endingId);
                }

                uiEnding.AutoFinish = false;
                uiEnding.NextEnding();
            }

            // generates a csv for all endings in the file EndingIds
            if (developerMode && Input.GetKeyDown(KeyCode.J))
            {
                // write the ending text into a file
                StreamWriter writer = new StreamWriter($"ModResources{directorySeparatorChar}EnglishModExtras{directorySeparatorChar}EndingTranslations.csv");
                writer.WriteLine("TextId,Translated,Original");

                foreach(string endingIdIter in endingIds)
                {
                    // Note, do not use <directorySeparatorChar> in Game.Resource.Load, it breaks Heluo's function
                    string pathEnding = $"UI/UIFrom/UIEnding_chs/{endingIdIter}.prefab";
                    GameObject endingObj = Game.Resource.Load<GameObject>(pathEnding);

                    if (endingObj == null)
                    {
                        continue;
                    }

                    // Location 1, endingObj.Text.Text00, single text, very commonly found as titles
                    Text titleText = endingObj.transform.Find("Text")?.Find("Text00")?.GetComponent<Text>();
                    if(titleText != null)
                    {
                        string titleStr = RemoveNewLine(titleText.text);
                        writer.WriteLine($"{endingIdIter} title,,\"{titleStr}\"");
                    }

                    // Location 2, endingObj.Text.Grouop, list of text, very commonly found as paragraph text
                    Transform lineParent = endingObj.transform.Find("Text")?.Find("Grouop");
                    if (lineParent != null)
                    {
                        foreach (Transform lineTran in lineParent)
                        {
                            string lineStr = RemoveNewLine(lineTran.GetComponent<Text>().text);
                            writer.WriteLine($"{endingIdIter} line {lineTran.name},,\"{lineStr}\"");
                        }
                    }

                    // Location 3, endingObj.Text.Grouop01, list of text, used when Grouop is full, found in ending0106f_in0209dead_sub
                    Transform lineParent2 = endingObj.transform.Find("Text")?.Find("Grouop01");
                    if (lineParent2 != null)
                    {
                        foreach (Transform lineTran2 in lineParent2)
                        {
                            string lineStr2 = RemoveNewLine(lineTran2.GetComponent<Text>().text);
                            writer.WriteLine($"{endingIdIter} line2 {lineTran2.name},,\"{lineStr2}\"");
                        }
                    }

                    // Location 4, endingObj, list of text, found in ending0104_b
                    foreach (Transform endingObjChild in endingObj.transform)
                    {
                        string cinematicText = RemoveNewLine(endingObjChild.GetComponent<Text>()?.text);
                        if(cinematicText != null)
                        {
                            writer.WriteLine($"{endingIdIter} cinematic {endingObjChild.name},,\"{cinematicText}\"");
                        }
                    }

                    // ending0103 does not have any text

                    Game.Resource.Unload(pathEnding);
                }

                writer.Close();
            }
        }

        // Removes new line characters from <text>. This is used to clean text.
        private static string RemoveNewLine(string text)
        {
            return text?.Replace("\n", "")?.Replace("\r", "");
        }

        /**
         * Sets <text> to Ariel with a font size of <fontSize>.
         * This function allows english text to fit more easily.
         * Note, overflow behaviours are not specified to keep unity's word wrapping and auto shrink feature.
         */
        private static void FormatText(Text text, int fontSize)
        {
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
        }

        /**
         * Parse <endingTranslationLine> and add the translation into <endingTranslationDict>
         * 
         * The csv is read line by line creating <endingTranslationLine>
         * <endingTranslationLine> holds certain properties, notably, it always contains 3 columns in the csv.
         * Column 1 contains the identifier
         * Column 2 contains the translated text
         * Column 3 contains the original text
         * Note that column 2 often contains special characters such as " and ,
         */
        private static void ParseCsvEndingTranslation(Dictionary<string, string> endingTranslationDict, string endingTranslationLine)
        {
            string[] columns = endingTranslationLine.Split(',');
            int columnNum = columns.Length;

            // if there are less than 3 split elements, the <endingTranslationLine> is guaranteed to be invalid.
            if (columnNum < 3)
            {
                return;
            }

            // craft the 2nd column by merging all excess splits. This is based off the assumption that column 2 is the only column that may contain commas
            string textId = columns[0];
            string textTranslation = "";

            for(int columnCurrent = 1; columnCurrent < columnNum - 1; columnCurrent++)
            {
                textTranslation += columns[columnCurrent];
                if(columnCurrent != columnNum - 2)
                {
                    textTranslation += ",";
                }
            }

            // in csv, " denotes that the cell contains special characters. Here is an example: "He says, ""You're finally here.""".
            // the below code processes the special characters. Using the above example, it produces: He says, "You're finally here."
            // NEWLINE substitutes for a new line character.
            if (textTranslation.StartsWith("\""))
            {
                textTranslation = textTranslation.Substring(1, textTranslation.Length - 2);
                textTranslation = textTranslation.Replace("\"\"", "\"");
            }

            textTranslation = textTranslation.Replace("NEWLINE", "\n");

            endingTranslationDict.Add(textId, textTranslation);
        }
    }
}
