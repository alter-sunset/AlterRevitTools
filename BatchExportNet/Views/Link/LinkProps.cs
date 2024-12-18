﻿using Autodesk.Revit.DB;

namespace AlterTools.BatchExportNet.Views.Link
{
    public class LinkProps(WorksetTable table, bool setWorksetId)
    {
        public WorksetTable WorksetTable { get; set; } = table;
        public bool SetWorksetId { get; set; } = setWorksetId;
    }
}