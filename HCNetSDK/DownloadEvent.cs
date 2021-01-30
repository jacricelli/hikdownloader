namespace HikDownloader.HCNetSDK
{
    using System;

    /// <summary>
    /// Evento de descarga.
    /// </summary>
    public class DownloadEvent : EventArgs
    {
        /// <summary>
        /// Obtiene un valor con la grabación.
        /// </summary>
        public Recording Recording { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        public DownloadEvent(Recording recording)
        {
            Recording = recording;
        }
    }
}
