﻿using System.ComponentModel;
using System.Windows;
using VLS.BatchExportNet.Source.EventHandlers;

namespace VLS.BatchExportNet.Views.Migrate
{
    /// <summary>
    /// Interaction logic for DetachModelsView.xaml
    /// </summary>
    public partial class MigrateModelsView : Window
    {
        public MigrateModelsView(EventHandlerMigrateModelsVMArg eventHandlerMigrateModelsVMArg)
        {
            InitializeComponent();
            DataContext = new MigrateViewModel(eventHandlerMigrateModelsVMArg);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DataContext = null;
            base.OnClosing(e);
        }
    }
}