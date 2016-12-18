#region Namespaces
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Events;

#endregion

namespace Sandwich
{
    public class Watcher {

        public string pendingJobFolder;
        public Queue<Job> jobQueue;

        public Watcher(string watchPath) {
            pendingJobFolder = watchPath;
            jobQueue = JobHelper.GetPendingJobs(pendingJobFolder);
    }

        // Keeps watcher from being garbage collected
        private static FileSystemWatcher watcher;

        public void watch()
        {

            // Adds Listener for Future Jobs
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = pendingJobFolder;
            //watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Filter = "*.*";
            watcher.EnableRaisingEvents = true;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //string pendingJobPath = e.FullPath;
            jobQueue = JobHelper.GetPendingJobs(pendingJobFolder);
            //_uiapp.OpenAndActivateDocument(jobRevitPath);
            //_app.LoadFile(jobRevitPath);
            //_uiapp.LoadFile(jobRevitPath);

        }

    }
}