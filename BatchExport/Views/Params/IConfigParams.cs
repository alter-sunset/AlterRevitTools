using AlterTools.BatchExport.Views.Base;

namespace AlterTools.BatchExport.Views.Params
{
    public interface IConfigParams : IConfigBase
    {
        public string[] ParametersNames { get; }
    }
}