using Newtonsoft.Json.Linq;

namespace AlterTools.DriveFromOutside.Events;

/// <summary>
///     Object that will hold configuration obtained from message
/// </summary>
public class TaskConfig
{
    /// <summary>
    ///     External Event to call
    /// </summary>
    public ExternalEvents ExternalEvent { get; set; }

    /// <summary>
    ///     Object with configuration if given event needs one
    /// </summary>
    public JToken? EventConfig { get; set; }

    /// <summary>
    ///     Path to the configuration file
    /// </summary>
    public string FilePath { get; set; } = string.Empty;
}