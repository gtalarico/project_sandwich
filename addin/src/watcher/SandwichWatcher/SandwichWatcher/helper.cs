#region Namespaces
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

#endregion

namespace Sandwich.Watcher
{
    public class JsonHelper {
        const string PendingFolder = @"C:\Users\gtalarico\Dropbox\Shared\dev\repos\project_sandwich\jobs\pending";

        public static HashSet<string> GetPaths(){

            string[] fileEntries = Directory.GetFiles(PendingFolder);
            HashSet<string> filePathList= new HashSet<string>();
            foreach (string fileName in fileEntries)
            {

                filePathList.Add(getPathFromJson(fileName));
            }

                return filePathList;
        }

        static string getPathFromJson(string jsonPath)
        {
            using (StreamReader reader = new StreamReader(jsonPath))
            {
                string json = reader.ReadToEnd();
                dynamic jsonData = JsonConvert.DeserializeObject(json);
                return jsonData.filepath;
            }
            
        }

    }
}