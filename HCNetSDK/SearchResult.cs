namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Resultado de búsqueda.
    /// </summary>
    public class SearchResult : SearchEvent
    {
        /// <summary>
        /// Obtiene un valor con la grabación.
        /// </summary>
        public Recording Recording { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Canal.</param>
        /// <param name="recording">Grabación.</param>
        public SearchResult(Channel channel, Recording recording): base(channel)
        {
            Recording = recording;
        }
    }
}
