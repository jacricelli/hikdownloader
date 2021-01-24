namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Error de búsqueda.
    /// </summary>
    public class SearchError : SearchEvent
    {
        /// <summary>
        /// Código.
        /// </summary>
        public uint Code { get; private set; } = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Canal.</param>
        /// <param name="code">Código.</param>
        public SearchError(Channel channel, uint code): base(channel)
        {
            Code = code;
        }
    }
}
