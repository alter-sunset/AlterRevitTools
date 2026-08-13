namespace AlterTools.atExportModel.Interfaces;

public interface IClassificationSettings
{
    string ClassificationName { get; set; }
    string ClassificationEdition { get; set; }
    string ClassificationSource { get; set; }
    string ClassificationEditionDate { get; set; } // example: "\/Date(1783285200000)\/"
    string ClassificationLocation { get; set; }
    string ClassificationFieldName { get; set; }
}