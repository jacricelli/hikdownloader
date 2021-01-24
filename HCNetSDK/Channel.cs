namespace HikDownloader.HCNetSDK
{
    /// <summary>
    /// Canal.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Número.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="number">Número.</param>
        public Channel(int number)
        {
            Number = number;
        }
    }
}
