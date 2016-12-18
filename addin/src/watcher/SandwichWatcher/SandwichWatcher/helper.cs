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

namespace Sandwich
{
    public class JobHelper {
        
        // Gets Json Data from Json Filepath
        public static Job GetJobJson(string jsonJobFilepath)
        {
            using (StreamReader reader = new StreamReader(jsonJobFilepath))
            {
                string json = reader.ReadToEnd();
                Job jobData = JsonConvert.DeserializeObject<Job>(json);
                return jobData;
            }

        }

        // Returns list of pending jobs
        public static Queue<Job> GetPendingJobs(string PendingJobPath)

        {
            string[] fileEntries = Directory.GetFiles(PendingJobPath);
            Queue<Job> filePathList = new Queue<Job>();

            foreach (string fileName in fileEntries)
            {
                Job job = GetJobJson(fileName);
                filePathList.Enqueue(job);
            }

            return filePathList;
        }


    }
}