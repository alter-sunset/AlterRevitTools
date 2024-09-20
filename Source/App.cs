﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.UI;
using System.Text.Json;
using VLS.BatchExportNet.Utils;
using System;

namespace VLS.BatchExportNet.Source
{
    public class App : IExternalApplication
    {
        const string BASE = "VLS.BatchExportNet.";
        public Result OnStartup(UIControlledApplication a)
        {
            const string TAB_NAME = "VLS";

            //Create default tab 
            try
            {
                a.CreateRibbonTab(TAB_NAME);
            }
            catch { }

            //Get buttons to create from json config
            ButtonContext[] buttons = GetButtonContext();

            //Create panels from config
            IEnumerable<Tuple<RibbonPanel, string>> panels = buttons
                .Select(e => e.Panel)
                .Distinct()
                .Select(e => new Tuple<RibbonPanel, string>(RibbonPanel(a, TAB_NAME, e), e));

            //Create buttons from config
            foreach (ButtonContext button in buttons)
            {
                CreateButton(button, panels);
            }

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;
        private static RibbonPanel RibbonPanel(UIControlledApplication a, string tabName, string panelName)
        {
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            }
            catch { }
            return a.GetRibbonPanels(tabName).FirstOrDefault(p => p.Name == panelName);
        }
        private static ButtonContext[] GetButtonContext()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("VLS.BatchExportNet.Resources.Buttons.json");
            JsonSerializerOptions options = JsonHelper.GetDefaultOptions();
            return JsonSerializer.Deserialize<ButtonContext[]>(stream, options);
        }
        private static void CreateButton(ButtonContext button, IEnumerable<Tuple<RibbonPanel, string>> panels)
        {
            RibbonPanel ribbonPanel = panels.First(e => e.Item2 == button.Panel).Item1;
            try
            {
                PushButtonData pushButtonData = button.GetPushButtonData(BASE);
                if (button.Availability)
                {
                    pushButtonData.AvailabilityClassName = BASE + "Source.CommandAvailability";
                }
                _ = ribbonPanel.AddItem(pushButtonData) as PushButton;
            }
            catch { }
        }
    }
}