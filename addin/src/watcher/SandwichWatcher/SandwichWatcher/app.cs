#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Reflection;

using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

#endregion

namespace Sandwich
{
    [Transaction(TransactionMode.Manual)]
    class app : IExternalApplication
    {

        // Globals
        static app _thisApp;
        private UIApplication _uiapp;

        //static string sandwichRunner = "CustomCtrl_%CustomCtrl_%Sandwich%Sandwich%SandwichSettings";
        static string sandwichRunnerCommandId = "CustomCtrl_%CustomCtrl_%Sandwich%Sandwich%SandwichRun";

        // Watcher
        Watcher _watcher = new Watcher(@"D:\Dropbox\Shared\dev\repos\project_sandwich\jobs\pending");

        // External Events
        ExternalEvent _exEvent;
        EventRegisterHandler _exeventHander;


        public Result OnStartup(UIControlledApplication uiControlledApp)
        {

            _thisApp = this;

            String assemblyPath = Assembly.GetExecutingAssembly().Location;
            String tabName = "Sandwich";

            uiControlledApp.CreateRibbonTab(tabName);
            RibbonPanel panel = uiControlledApp.CreateRibbonPanel(tabName, "Sandwich");

            PushButtonData buttonData = new PushButtonData("SandwichSettings", "Settings", assemblyPath, "Sandwich.Settings");
            PushButton pushbutton = panel.AddItem(buttonData) as PushButton;

            PushButtonData buttonData2 = new PushButtonData("SandwichRun", "Run", assemblyPath, "Sandwich.RunJob");
            PushButton pushbutton2 = panel.AddItem(buttonData2) as PushButton;



            #region IconbyPath
            //String iconPath = @"D:\Dropbox\Shared\dev\repos\project_sandwich\addin\src\watcher\SandwichWatcher\SandwichWatcher\sandwich.png";
            //Uri uriImage = new Uri(iconPath);
            //BitmapImage largeImage = new BitmapImage(uriImage);
            //pushbutton.LargeImage = largeImage;
            #endregion
            #region Icon by Resource
            BitmapImage pbImage = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/sandwich.png"));
            pushbutton.LargeImage = pbImage;
            pushbutton2.LargeImage = pbImage;
            #endregion


            #region Retrieving UIApplication from UIControlledApplication
            Type type = uiControlledApp.GetType();

            BindingFlags flags = BindingFlags.Public
                               | BindingFlags.NonPublic
                               | BindingFlags.GetProperty
                               | BindingFlags.Instance;

            MemberInfo[] propertyMembers = type.GetMembers(flags);

            // Note that the field "m_application" is listed
            // in the propertyMembers array, and also the 
            // method "getUIApp"... let's grab the field:

            string propertyName = "m_application";
            flags = BindingFlags.Public
                  | BindingFlags.NonPublic
                  | BindingFlags.GetField
                  | BindingFlags.Instance;

            Binder binder = null;
            object[] args = null;
            object result = type.InvokeMember(propertyName, flags, binder, uiControlledApp, args);
            UIApplication _uiapp;
            _uiapp = result as UIApplication;

            #endregion // Retrieving UIApplication from UIControlledApplication
            uiControlledApp.ControlledApplication.ApplicationInitialized += OnApplicationInitialized;
            uiControlledApp.ControlledApplication.DocumentOpened += OnDocumentOpened;
            uiControlledApp.Idling += OnIdling;

            // External Event
            ExternalEvent _exEvent;
            EventRegisterHandler _exeventHander = new EventRegisterHandler();

            return Result.Succeeded;
        }

        void OnApplicationInitialized(object sender, ApplicationInitializedEventArgs e)
        {
           // Sender is an Application instance:
            Application app = sender as Application;
            _uiapp = new UIApplication(app);
            //TaskDialog.Show("Sandwich", "Sandwich Loaded");
            _watcher.watch();
            
            string placeHolder = @"D:\\Dropbox\\Shared\\dev\\repos\\project_sandwich\\revit\\placeholder.rvt";
            _uiapp.OpenAndActivateDocument(placeHolder);
            LoadFile(_uiapp, placeHolder);

        // Get the command id for the import command - Got it from Journal File
        // RevitCommandId commandId = RevitCommandId.LookupCommandId("CustomCtrl_%CustomCtrl_%Add-Ins%RevitPythonShell%runner");
        //_uiapp.PostCommand(commandId);
        }

        void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {

        }

            void OnIdling (object sender, IdlingEventArgs e)
        {

            //Sets the next invocation of the idling event to be called promptly
            e.SetRaiseWithoutDelay();

            //Application app = sender as Application;
            
            while (_watcher.jobQueue.Count > 0)
            {
                Job job = _watcher.jobQueue.Dequeue();
                string job_filepath = Path.Combine(_watcher.pendingJobFolder, job.job_id);
                UIDocument uidoc = LoadFile(_uiapp, job.filepath);

                if (uidoc != null)
                {
                    string done_job_filepath = Path.Combine(@"D:\Dropbox\Shared\dev\repos\project_sandwich\jobs", job.job_id);
                    File.Move(job_filepath, done_job_filepath);

                    //try
                    //{
                    //    //RevitCommandId sandwichRunnerCommand = RevitCommandId.LookupCommandId(sandwichRunnerCommandId);
                    //    //_uiapp.PostCommand(sandwichRunnerCommand);

                    //}
                    //catch (Exception error)
                    //{
                    //    TaskDialog.Show("Weird Error", String.Format("Error Posting Command: {0}", error.Message));
                    //}

                    EventRegisterHandler _exeventHander = new EventRegisterHandler();
                    _exeventHander.path = job.filepath;
                    _exEvent = ExternalEvent.Create(_exeventHander);
                    _exEvent.Raise();


                    // TODO: Close Document - Cannot close from within event...?
                    //string placeHolderModel = @"D:\Dropbox\Shared\dev\repos\project_sandwich\revit\placeholder.rvt";
                    //UIDocument placeHolderDoc = LoadFile(placeHolderModel);
                    //uidoc.Document.Close();
                    //System.Windows.Forms.SendKeys.SendWait("^{F4}");

                }            
            }

            //if (app != null)
            //{
            //    UIApplication uiapp = new UIApplication(app);
            //    UIDocument uidoc = uiapp.ActiveUIDocument;
            //    Document doc = uidoc.Document;

            //}
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        public static UIDocument LoadFile(UIApplication uiapp, string filePath)
        {
            UIDocument uidoc = null;
            try
            {
                uidoc = uiapp.OpenAndActivateDocument(filePath);
                return uidoc;
            }
            catch (Exception error)
            {
                string errorMessage = error.Message;
                TaskDialog.Show("Weird Error", String.Format("Error Opening File: {0} \n\r {1}", error.Message, filePath));
                return uidoc;
            }
        }

    }
}