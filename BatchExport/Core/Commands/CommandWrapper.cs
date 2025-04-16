﻿using AlterTools.BatchExport.Utils;
using Autodesk.Revit.UI;
using System;

namespace AlterTools.BatchExport.Core.Commands
{
    static class CommandWrapper
    {
        internal static Result Execute(ref string msg, Forms form, UIApplication uiApp = null)
        {
            try
            {
                form.ShowForm(uiApp);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return Result.Failed;
            }
        }
    }
}