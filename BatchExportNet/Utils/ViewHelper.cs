using System;
using System.Windows;
using VLS.BatchExportNet.Source;
using VLS.BatchExportNet.Views.IFC;
using VLS.BatchExportNet.Views.NWC;
using VLS.BatchExportNet.Views.Link;
using VLS.BatchExportNet.Views.Detach;
using VLS.BatchExportNet.Views.Migrate;
using VLS.BatchExportNet.Views.Transmit;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Utils
{
    static class ViewHelper
    {
        private static Window _myForm;
        internal static void ShowForm(this Forms form)
        {
            if (_myForm != null)
            {
                _myForm.Close();
                _myForm = null;
            }

            try
            {
                switch (form) //might need to rework this aproach if i need multiple views fsr
                {
                    case Forms.Detach:
                        EventHandlerDetachModelsVMArg evDetachVM = new();
                        _myForm = new DetachModelsView(evDetachVM);
                        break;
                    case Forms.IFC:
                        EventHandlerIFCExportVMArg evIFC_VM = new();
                        _myForm = new IFCExportView(evIFC_VM);
                        break;
                    case Forms.NWC:
                        EventHandlerNWCExportVMArg evNWC_VM = new();
                        EventHandlerNWCExportBatchVMArg evBatchNWC_VM = new();
                        _myForm = new NWCExportView(evNWC_VM, evBatchNWC_VM);
                        break;
                    case Forms.Migrate:
                        EventHandlerMigrateModelsVMArg evMigrateVM = new();
                        _myForm = new MigrateModelsView(evMigrateVM);
                        break;
                    case Forms.Transmit:
                        EventHandlerTransmitModelsVMArg evTransmitVM = new();
                        _myForm = new TransmitModelsView(evTransmitVM);
                        break;
                    case Forms.Link:
                        EventHandlerLinkModelsVMArg evLinkVM = new();
                        _myForm = new LinkModelsView(evLinkVM);
                        break;
                    default:
                        _myForm = null;
                        return;
                }
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}