namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Progreso de descarga.
    /// </summary>
    public class DownloadProgress : DownloadEvent
    {
        /// <summary>
        /// Progreso.
        /// </summary>
        public int Progress { get; private set; } = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="progress">Progreso.</param>
        public DownloadProgress(Recording recording, int progress) : base(recording)
        {
            Progress = progress;
        }
    }
}