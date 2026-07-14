using JetBrains.Annotations;

namespace AlterTools.Utils.Interfaces;

public interface IConfigBase
{
    string[] Files { get; }

    [UsedImplicitly] string ViewName { get; set; }

    [UsedImplicitly] string FolderPath { get; set; }
}