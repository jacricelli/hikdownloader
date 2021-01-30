namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Error de descarga.
    /// </summary>
    public class DownloadError : DownloadEvent
    {
        /// <summary>
        /// Código.
        /// </summary>
        public uint Code { get; private set; } = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="code">Código.</param>
        public DownloadError(Recording recording, uint code) : base(recording)
        {
            Code = code;
        }
    }
}