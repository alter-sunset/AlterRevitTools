using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.IFC;
using AlterTools.BatchExport.Views.NWC;
using AlterTools.BatchExport.Views.Link;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.Migrate;
using AlterTools.BatchExport.Views.Transmit;
using AlterTools.BatchExport.Views.Params;
using AlterTools.BatchExport.Core.Commands;

namespace AlterTools.BatchExport.Utils
{
    internal static class ViewHelper
    {
        private static UIApplication _uiApp;
        private static Window _myForm;

        private static readonly Dictionary<Forms, Func<Window>> FormCreators = new()
        {
            { Forms.Detach,     () => new DetachModelsView(     new EventHandlerDetach()                            )},
            { Forms.Ifc,        () => new IfcExportView(        new EventHandlerIfc()                               )},
            { Forms.Nwc,        () => new NwcExportView(        new EventHandlerNwc(), new EventHandlerNwcBatch()  )},
            { Forms.Migrate,    () => new MigrateModelsView(    new EventHandlerMigrate()                           )},
            { Forms.Transmit,   () => new TransmitModelsView(   new EventHandlerTransmit()                          )},
            { Forms.Link,       () => new LinkModelsView(       new EventHandlerLink(), GetWorksets()               )},
            { Forms.Params,     () => new ExportParamsView(     new EventHandlerParams()                            )}
        };

        internal static void ShowForm(this Forms form, UIApplication uiApp)
        {
            CloseCurrentForm();

            _uiApp = uiApp;

            try
            {
                if (!FormCreators.TryGetValue(form, out var createForm)) return;

                _myForm = createForm();
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static Workset[] GetWorksets()
        {
            return new FilteredWorksetCollector(_uiApp.ActiveUIDocument.Document)
                       .OfKind(WorksetKind.UserWorkset)
                       .ToWorksets()
                       .ToArray();
        }

        private static void CloseCurrentForm()
        {
            if (null == _myForm) return;

            _myForm.Close();
            _myForm = null;
        }
    }
}