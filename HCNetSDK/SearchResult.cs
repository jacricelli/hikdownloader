namespace HikDownloader.HCNetSDK
{
    using System;

    /// <summary>
    /// Resultado de búsqueda.
    /// </summary>
    public class SearchResult : EventArgs
    {
        /// <summary>
        /// Obtiene un valor con la grabación.
        /// </summary>
        public Recording Recording { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        public SearchResult(Recording recording)
        {
            Recording = recording;
        }
    }
}
