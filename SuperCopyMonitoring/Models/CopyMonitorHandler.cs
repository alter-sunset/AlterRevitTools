using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Reference = Autodesk.Revit.DB.Reference;

namespace SuperCopyMonitoring.Models
{
    public class CopyMonitorHandler
    {
        private readonly UIApplication _uiApp;
        private readonly UIDocument _uiDoc;
        private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly Document _doc;

        public CopyMonitorHandler(UIApplication uiApp)
        {
            _uiApp = uiApp;
            _uiDoc = _uiApp.ActiveUIDocument;
            _app = _uiApp.Application;
            _doc = _uiDoc.Document;
        }

        public void CopySelected()
        {
            XYZ zeroPoint = new();
            IList<Reference> selectedElements;
            try
            {
                selectedElements = _uiDoc.Selection
                    .PickObjects(ObjectType.LinkedElement, new ElementsSelectionFilter(_doc), "Выберите элементы, которые необходимо скопировать и замониторить");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Отмена", ex.Message);
                return;
            }

            Dictionary<ElementId, List<ElementId>> selectedPairs = selectedElements
                .Select(r => new
                {
                    Key = r.ElementId,
                    Element = r.LinkedElementId
                })
                .Where(r => r is not null)
                .GroupBy(r => r.Key)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Element).ToList());

            CopyPasteOptions copyPasteOptions = new();
            copyPasteOptions.SetDuplicateTypeNamesHandler(new DuplicateAction());

            using Transaction t = new(_doc);
            t.Start("Копия элементов из связи");

            foreach (KeyValuePair<ElementId, List<ElementId>> link in selectedPairs)
            {
                RevitLinkInstance revitLinkInstance = _doc.GetElement(link.Key) as RevitLinkInstance;
                Document linkDocument = revitLinkInstance.GetLinkDocument();
                Transform transform = revitLinkInstance.GetTotalTransform();

                IList<ElementId> sortedElements = link.Value
                    .Where(e => linkDocument.GetElement(e).Category.CategoryType == CategoryType.Model)
                    .ToList();

                IList<ElementId> copiedElements = ElementTransformUtils.CopyElements(linkDocument, sortedElements, _doc, transform, copyPasteOptions).ToList();

                for (int i = 0; i < copiedElements.Count; i++)
                {
                    Element newElement = _doc.GetElement(copiedElements[i]);
                    if (newElement is null) continue;

                    //fill db
                }
            }

            t.Commit();
        }
    }
}
