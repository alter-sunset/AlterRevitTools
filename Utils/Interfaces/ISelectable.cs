using JetBrains.Annotations;

namespace AlterTools.Utils.Interfaces;

public interface ISelectable
{
    [UsedImplicitly] bool IsSelected { get; set; }
}