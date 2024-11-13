﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using VLS.BatchExportNet.Source;
using VLS.BatchExportNet.Source.EventHandlers;
using VLS.BatchExportNet.Views.IFC;
using VLS.BatchExportNet.Views.NWC;
using VLS.BatchExportNet.Views.Link;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Migrate;
using VLS.BatchExportNet.Views.Transmit;

namespace VLS.BatchExportNet.Utils
{
    static class ViewHelper
    {
        private static Window _myForm;
        private static UIApplication _uiApp;
        private static readonly Dictionary<Forms, Func<Window>> _formCreators = new()
        {
            { Forms.Detach,
                () => new DetachModelsView(new EventHandlerDetach()) },
            { Forms.IFC,
                () => new IFCExportView(new EventHandlerIFC()) },
            { Forms.NWC,
                () => new NWCExportView(new EventHandlerNWC(), new EventHandlerNWC_Batch()) },
            { Forms.Migrate,
                () => new MigrateModelsView(new EventHandlerMigrate()) },
            { Forms.Transmit,
                () => new TransmitModelsView(new EventHandlerTransmit()) },
            { Forms.Link,
                () => new LinkModelsView(new EventHandlerLink(), GetWorksets()) },
        };

        internal static void ShowForm(this Forms form, UIApplication uiApp)
        {
            CloseCurrentForm();
            _uiApp = uiApp;

            try
            {
                if (_formCreators.TryGetValue(form, out var createForm))
                {
                    _myForm = createForm();
                    _myForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static Workset[] GetWorksets() =>
            new FilteredWorksetCollector(_uiApp.ActiveUIDocument.Document)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets()
                .ToArray();

        private static void CloseCurrentForm()
        {
            if (_myForm is null) return;
            _myForm.Close();
            _myForm = null;
        }
    }
}