using Autodesk.Revit.UI.Selection;

namespace SuperCopyMonitoring.Models
{
    public class ElementsSelectionFilter : ISelectionFilter
    {
        private readonly Document _doc;
        public ElementsSelectionFilter(Document document)
        {
            _doc = document ?? throw new ArgumentNullException(nameof(document));
        }

        public bool AllowElement(Element element) => element is not null;

        public bool AllowReference(Reference reference, XYZ point)
        {
            if (reference is null) return false;

            if (_doc.GetElement(reference) is not RevitLinkInstance revitLinkInstance) return false;

            Document linkedDocument = revitLinkInstance.GetLinkDocument();
            if (linkedDocument is null) return false;

            Element element = linkedDocument.GetElement(reference.LinkedElementId);
            return element is not null;
        }
    }
}