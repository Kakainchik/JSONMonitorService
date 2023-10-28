namespace FileMonitorService
{
    public interface IFileMonitor<T> : IDisposable
    {
        /// <summary>
        /// Path to the full location of the file.
        /// </summary>
        string FilePath { get; }
        /// <summary>
        /// Desserialized data of the file.
        /// </summary>
        T Data { get; }

        event EventHandler<bool>? DataChecked;

        /// <summary>
        /// Start monitoring for any changes in the file.
        /// </summary>
        void StartMonitoring();
        /// <summary>
        /// Cease monitoring for changes.
        /// </summary>
        void StopMonitoring();
        /// <summary>
        /// Initiate checking for changes.
        /// </summary>
        /// <returns><c>True</c> if changes found, otherwise <c>False</c>.</returns>
        bool ForceCheck();
    }
}