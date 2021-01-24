namespace HikDownloader.HCNetSDK
{
    using System;

    /// <summary>
    /// Evento de búsqueda.
    /// </summary>
    public class SearchEvent : EventArgs
    {
        /// <summary>
        /// Canal.
        /// </summary>
        public Channel Channel { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Canal.</param>
        public SearchEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
