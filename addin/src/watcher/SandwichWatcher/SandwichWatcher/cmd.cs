#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace Sandwich
{
    [Transaction(TransactionMode.Manual)]
    public class RunJob : IExternalCommand
    {

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            TaskDialog.Show("Sandwich", "Sandwich Runner Ran");

            //ExternalEvent _exEvent;
            //EventRegisterHandler _exeventHander = new EventRegisterHandler();
            //_exEvent = ExternalEvent.Create(_exeventHander);
            //_exEvent.Raise();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class Settings : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            TaskDialog.Show("Sandwich", "Settings Ran");
            // Settings Class
            return Result.Succeeded;
        }
    }

    public class EventRegisterHandler : IExternalEventHandler
    {
        public string path;

        public void Execute(UIApplication uiapp)
        {
            TaskDialog.Show("External Event", "Click Close to close.");
            string placeHolderpath = @"D:\Dropbox\Shared\dev\repos\project_sandwich\revit\placeholder.rvt";
            app.LoadFile(uiapp, path);

            string sandwichRunnerCommandId = "CustomCtrl_%CustomCtrl_%Sandwich%Sandwich%SandwichRun";
            RevitCommandId sandwichRunnerCommand = RevitCommandId.LookupCommandId(sandwichRunnerCommandId);
            uiapp.PostCommand(sandwichRunnerCommand);

            app.LoadFile(uiapp, placeHolderpath);

            DocumentSetIterator openDocs = uiapp.Application.Documents.ForwardIterator();
            openDocs.Reset();
            while (openDocs.MoveNext())
            {
                Document doc = openDocs.Current as Document;
                string title = doc.Title;
                if (title != "placeholder.rvt"){
                    doc.Close();
                }

            }
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }
}