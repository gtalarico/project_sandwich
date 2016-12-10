#region Namespaces
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Reflection;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

#endregion

namespace Sandwich.Watcher
{
    [Transaction(TransactionMode.Manual)]
    class app : IExternalApplication
    {
        
        //UIApplication _uiapp;

        public Result OnStartup(UIControlledApplication a)
        {

            //JObject o1 = JObject.Parse(File.ReadAllText(@"c:\videogames.json"));
            // read JSON directly from a file
            //using (StreamReader file = File.OpenText(@"c:\videogames.json"))
            //using (JsonTextReader reader = new JsonTextReader(file))
            //{
            //  JObject o2 = (JObject) JToken.ReadFrom(reader);
            //}

            #region Retrieving UIApplication from UIControlledApplication
            Type type = a.GetType();

            BindingFlags flags = BindingFlags.Public
              | BindingFlags.NonPublic
              | BindingFlags.GetProperty
              | BindingFlags.Instance;

            MemberInfo[] propertyMembers = type.GetMembers(
              flags);

            // Note that the field "m_application" is listed
            // in the propertyMembers array, and also the 
            // method "getUIApp"... let's grab the field:

            string propertyName = "m_application";
            flags = BindingFlags.Public | BindingFlags.NonPublic
              | BindingFlags.GetField | BindingFlags.Instance;
            Binder binder = null;
            object[] args = null;

            object result = type.InvokeMember(
                propertyName, flags, binder, a, args);

            UIApplication _uiapp;

            _uiapp = (UIApplication)result;
            #endregion // Retrieving UIApplication from UIControlledApplication

            a.ControlledApplication.ApplicationInitialized
              += OnApplicationInitialized;

            return Result.Succeeded;
        }

        void OnApplicationInitialized(object sender, ApplicationInitializedEventArgs e)
        {
           // Sender is an Application instance:

            Application app = sender as Application;
            UIApplication uiapp = new UIApplication(app);

            // Get the command id for the import command - Got it from Journal File
            RevitCommandId commandId = RevitCommandId.LookupCommandId("CustomCtrl_%CustomCtrl_%Add-Ins%RevitPythonShell%runner");

            //const string _test_project_filepath = @"C:\Users\gtalarico\Dropbox\Shared\dev\repos\project_sandwich\revit\File1.rvt";
            //HashSet<string> filPathList = JsonHelper.GetPaths();
            //foreach (string fileName in filPathList)
            //{
            //uiapp.OpenAndActivateDocument(fileName);
            uiapp.PostCommand(commandId);
            //}

            

        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}