using VLS.DriveFromOutside.Events;

namespace VLS.DriveFromOutside.Utils
{
    public interface IExternalTaskHandler
    {
        /// <summary>
        /// Task that will periodicaly check for new messages
        /// </summary>
        Task ListenForNewTasks(TimeSpan period);
        /// <summary>
        /// Method that should get messages and parse them to configs
        /// </summary>
        public List<TaskConfig> ReadMessages();
    }
}