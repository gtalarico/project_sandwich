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

namespace Sandwich.Watcher
{
    [Transaction(TransactionMode.Manual)]
    public class cmd : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Settings Class
           // RevitCommandHelper m_settings = new RevitCommandHelper(commandData);

            // Load Form
            //form_Main m_form = new form_Main(m_settings);
            //m_form.ShowDialog();

            return Result.Succeeded;
        }
    }
}