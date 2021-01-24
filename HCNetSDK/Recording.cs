namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Grabación.
    /// </summary>
    public class Recording
    {
        /// <summary>
        /// Canal.
        /// </summary>
        public Channel Channel { get; private set; }

        /// <summary>
        /// Video.
        /// </summary>
        public Video Video { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="channel">Canal.</param>
        /// <param name="video">Video.</param>
        public Recording(Channel channel, Video video)
        {
            Channel = channel;
            Video = video;
        }
    }
}
