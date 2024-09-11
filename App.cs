﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;
using VLS.BatchExportNet.Utils;
using VLS.BatchExportNet.NWC;
using VLS.BatchExportNet.IFC;
using VLS.BatchExportNet.Detach;
using VLS.BatchExportNet.Transmit;
using VLS.BatchExportNet.Migrate;
using VLS.BatchExportNet.Link;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace VLS.BatchExportNet
{
    public class App : IExternalApplication
    {
        private static Window _myForm;
        private string _thisAssemblyPath;

        public Result OnStartup(UIControlledApplication a)
        {
            _thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            RibbonPanel panelExtern = RibbonPanel(a, "Пакетный экспорт");
            string[] applIconPath = { "VLS.png", "VLS_16.png" };
            string[] ifcIconPath = { "ifc.png", "ifc_16.png" };
            string[] navisIconPath = { "navisworks.png", "navisworks_16.png" };

            RibbonPanel panelIntern = RibbonPanel(a, "Внутренние штуки");
            string[] rvtIconPath = { "rvt.png", "rvt_16.png" };

            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.IFC), "Пакетный экспорт в IFC", ifcIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.NWC), "Пакетный экспорт в NWC", navisIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Detach), "Пакетный экспорт отсоединённых моделей", applIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Migrate), "Пакетная миграция моделей с обновлением связей", applIconPath);
            CreateNewPushButton(panelExtern, PushButtonDataWrapper(Forms.Transmit), "Пакетная передача моделей", applIconPath);

            CreateNewPushButton(panelIntern, PushButtonDataWrapper(Forms.Link), "Пакетное добавление Revit ссылок", rvtIconPath);

            return Result.Succeeded;
        }
        private static void CreateNewPushButton(RibbonPanel ribbonPanel, PushButtonData pushButtonData, string toolTip, string[] iconPath)
        {
            const string BASEPATH = "VLS.BatchExportNet.Resources.";
            BitmapSource bitmap_32 = GetEmbeddedImage(BASEPATH + iconPath[0]);
            BitmapSource bitmap_16 = GetEmbeddedImage(BASEPATH + iconPath[1]);
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;
            pushButton.ToolTip = toolTip;
            pushButton.Image = bitmap_16;
            pushButton.LargeImage = bitmap_32;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        public static void ShowForm(UIApplication uiapp, Forms form)
        {
            if (_myForm != null && _myForm == null) return;

            try
            {
                switch (form)
                {
                    case Forms.Detach:
                        EventHandlerDetachModelsUiArg evDetachUi = new();
                        _myForm = new DetachModelsUi(uiapp, evDetachUi) { Height = 600, Width = 800 };
                        break;
                    case Forms.IFC:
                        EventHandlerIFCExportUiArg evIFCUi = new();
                        _myForm = new IFCExportUi(uiapp, evIFCUi) { Height = 700, Width = 800 };
                        break;
                    case Forms.NWC:
                        EventHandlerNWCExportUiArg evNWCUi = new();
                        EventHandlerNWCExportBatchUiArg eventHandlerNWCExportBatchUiArg = new();
                        _myForm = new NWCExportUi(uiapp, evNWCUi, eventHandlerNWCExportBatchUiArg) { Height = 900, Width = 800 };
                        break;
                    case Forms.Migrate:
                        EventHandlerMigrateModelsUiArg evMigrateUi = new();
                        _myForm = new MigrateModelsUi(uiapp, evMigrateUi) { Height = 200, Width = 600 };
                        break;
                    case Forms.Transmit:
                        EventHandlerTransmitModelsUiArg evTransmitUi = new();
                        _myForm = new TransmitModelsUi(uiapp, evTransmitUi) { Height = 500, Width = 800 };
                        break;
                    case Forms.Link:
                        EventHandlerLinkModelsUiArg evLinkUi = new();
                        _myForm = new LinkModelsUi(uiapp, evLinkUi) { Height = 500, Width = 800 };
                        break;
                }
                _myForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private static RibbonPanel RibbonPanel(UIControlledApplication a, string tabName)
        {
            string tab = "VLS";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch { }
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, tabName);
            }
            catch { }
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == tabName))
            {
                ribbonPanel = p;
            }
            return ribbonPanel;
        }
        private PushButtonData PushButtonDataWrapper(Forms forms)
        {
            const string BASE = "VLS.BatchExportNet.";
            string name = "";
            string text = "";
            string className = "";
            switch (forms)
            {
                case Forms.Detach:
                    name = "Экспорт отсоединённых моделей";
                    text = "Экспорт\nотсоединённых\nмоделей";
                    className = BASE + "Detach.ExportModelsDetached";
                    break;
                case Forms.IFC:
                    name = "Экспорт IFC";
                    text = "Экспорт\nIFC";
                    className = BASE + "IFC.ExportModelsToIFC";
                    break;
                case Forms.NWC:
                    name = "Экспорт NWC";
                    text = "Экспорт\nNWC";
                    className = BASE + "NWC.ExportModelsToNWC";
                    break;
                case Forms.Migrate:
                    name = "Миграция моделей";
                    text = "Миграция\nмоделей";
                    className = BASE + "Migrate.MigrateModels";
                    break;
                case Forms.Transmit:
                    name = "Передача моделей";
                    text = "Передача\nмоделей";
                    className = BASE + "Transmit.ExportModelsTransmitted";
                    break;
                case Forms.Link:
                    return new PushButtonData(
                            "Batch add Revit links",
                            "Batch add\nRevit Links",
                            _thisAssemblyPath,
                            BASE + "Link.LinkModels");
            }
            return new PushButtonData(name, text, _thisAssemblyPath, className)
            {
                AvailabilityClassName = BASE + "CommandAvailabilityWrapper"
            };
        }
        private static BitmapSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Stream s = a.GetManifestResourceStream(name);
                return BitmapFrame.Create(s);
            }
            catch
            {
                return null;
            }
        }
    }
    public enum Forms
    {
        Detach,
        IFC,
        NWC,
        Migrate,
        Transmit,
        Link
    }
    public class CommandAvailabilityWrapper : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories) => true;
    }
}