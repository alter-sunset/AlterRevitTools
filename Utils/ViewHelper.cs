using System;
using System.Windows;
using VLS.BatchExportNet.Source;
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
        internal static void ShowForm(Forms form)
        {
            if (_myForm != null && _myForm == null) return;

            try
            {
                switch (form)
                {
                    case Forms.Detach:
                        EventHandlerDetachModelsUiArg evDetachUi = new();
                        _myForm = new DetachModelsUi(evDetachUi) { Height = 550, Width = 800 };
                        break;
                    case Forms.IFC:
                        EventHandlerIFCExportUiArg evIFCUi = new();
                        _myForm = new IFCExportUi(evIFCUi) { Height = 700, Width = 800 };
                        break;
                    case Forms.NWC:
                        EventHandlerNWCExportUiArg evNWCUi = new();
                        EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg = new();
                        _myForm = new NWCExportUi(evNWCUi, eventHandlerNWCExportBatchUiArg) { Height = 900, Width = 800 };
                        break;
                    case Forms.Migrate:
                        EventHandlerMigrateModelsUiArg evMigrateUi = new();
                        _myForm = new MigrateModelsUi(evMigrateUi) { Height = 200, Width = 600 };
                        break;
                    case Forms.Transmit:
                        EventHandlerTransmitModelsUiArg evTransmitUi = new();
                        _myForm = new TransmitModelsUi(evTransmitUi) { Height = 500, Width = 800 };
                        break;
                    case Forms.Link:
                        EventHandlerLinkModelsUiArg evLinkUi = new();
                        _myForm = new LinkModelsUi(evLinkUi) { Height = 450, Width = 800 };
                        break;
                }
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}