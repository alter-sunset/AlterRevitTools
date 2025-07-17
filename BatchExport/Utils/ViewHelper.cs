using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AlterTools.BatchExport.Core.Commands;
using AlterTools.BatchExport.Core.EventHandlers;
using AlterTools.BatchExport.Views.Detach;
using AlterTools.BatchExport.Views.IFC;
using AlterTools.BatchExport.Views.Link;
using AlterTools.BatchExport.Views.Migrate;
using AlterTools.BatchExport.Views.NWC;
using AlterTools.BatchExport.Views.Params;
using AlterTools.BatchExport.Views.Transmit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AlterTools.BatchExport.Utils;

internal static class ViewHelper
{
    private static UIApplication _uiApp;
    private static Window _myForm;

    private static readonly Dictionary<Forms, Func<Window>> FormCreators = new()
    {
        { Forms.Detach, () => new DetachModelsView(new EventHandlerDetach()) },
        { Forms.IFC, () => new IFCExportView(new EventHandlerIFC()) },
        { Forms.NWC, () => new NWCExportView(new EventHandlerNWC(), new EventHandlerNWCBatch()) },
        { Forms.Migrate, () => new MigrateModelsView(new EventHandlerMigrate()) },
        { Forms.Transmit, () => new TransmitModelsView(new EventHandlerTransmit()) },
        { Forms.Link, () => new LinkModelsView(new EventHandlerLink(), GetWorksets()) },
        { Forms.Params, () => new ExportParamsView(new EventHandlerParams()) }
    };

    internal static void ShowForm(this Forms form, UIApplication uiApp)
    {
        CloseCurrentForm();

        _uiApp = uiApp;

        try
        {
            if (!FormCreators.TryGetValue(form, out Func<Window> createForm)) return;

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
        if (_myForm is null) return;

        _myForm.Close();
        _myForm = null;
    }
}