using AlterTools.BatchExport.Views.Base;
using AlterTools.Utils.Interfaces;

namespace AlterTools.BatchExport.Core.EventHandlers;

public abstract class EventHandlerBase : RevitEventWrapper<IConfigBase>
{
    protected abstract override void Execute(UIApplication app, ViewModelBase args);
}