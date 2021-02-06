using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace PoW_Tool_SheetUtilities
{
    internal class GoogleSheetConnector
    {
        //Singleton
        private static GoogleSheetConnector __Instance = null;

        public static GoogleSheetConnector GetInstance()
        {
            if (__Instance == null)
            {
                __Instance = new GoogleSheetConnector();
            }

            return __Instance;
        }

        private string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private string ApplicationName = "PoW_Tool_SheetUtilities";

        public Dictionary<string, string> SpreadsheetIDs;
        public Dictionary<string, string> SpreadsheetIDInts;
        public SheetsService Service = null;

        private GoogleSheetConnector()
        {
            Console.WriteLine("Connecting to Google Sheets API...");
            //Reading credentials
            UserCredential credential;

            //One needs a credentials.json that has edit access to the sheets inside the Outputs/Tools/SheetUtilties folder
            //See https://developers.google.com/sheets/api/quickstart/dotnet
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.jsson stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Console.WriteLine("Connected!");

            //Filling SpreadsheetIDs dictionary with key name and value ID
            SpreadsheetIDs = new Dictionary<string, string>();
            SpreadsheetIDs.Add("Talk", "1lmIjyewgIZfy2tqanCz1h1aPaKDcsbMOKOu1W5sjQzw");
            SpreadsheetIDs.Add("StandardizedTerms", "1GVyGWijCuuSlgkyOzTLwutRiS-mlCf5Lc35Y-mY7TR4");
            SpreadsheetIDs.Add("Achievement", "1M4b0Io5_KMvyoobHybAqrGJdU_qfrmh74U4OPT_k7dM");
            SpreadsheetIDs.Add("Adjustment", "1y32hg87yVTc67Bay6pF_ZhMPVf6lo1UpEULwZOn92o8");
            SpreadsheetIDs.Add("Alchemy", "1mEpb_0IGXA-DlU30uz6kLZye4fzCqA7Dga9Qqg1YEgk");
            SpreadsheetIDs.Add("Skill", "1DgoiJ-o9cJhZI9mNhpi7XK38DGAkZrb__fODcUUGDRU");
            SpreadsheetIDs.Add("Buffer", "149AKUIe9dudiedmI99tGd0WxC-YmE30TgaZYEo2vNyg");
        }
    }
}