using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Core.EventHandlers;

public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
{
    protected abstract override void Execute(UIApplication app, IConfigBase args);
}