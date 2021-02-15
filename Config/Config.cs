namespace HikDownloader.Config
{
    /// <summary>
    /// Configuración.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Configuración de HCNetSDK.
        /// </summary>
        public HCNetSDKConfig HCNetSDK { get; set; }

        /// <summary>
        /// Configuración de HikDownloader.
        /// </summary>
        public HikDownloaderConfig HikDownloader { get; set; }

        /// <summary>
        /// Configuración de FFmpeg.
        /// </summary>
        public FFmpegConfig FFmpeg { get; set; }
    }
}
