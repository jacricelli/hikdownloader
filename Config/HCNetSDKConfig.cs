namespace HikDownloader.Config
{
    /// <summary>
    /// Configuración de HCNetSDK.
    /// </summary>
    public class HCNetSDKConfig
    {
        /// <summary>
        /// Configuración del registro de mensajes.
        /// </summary>
        public HCNetSDKLogConfig Log { get; set; }

        /// <summary>
        /// Configuración de la sesión.
        /// </summary>
        public HCNetSDKSessionConfig Session { get; set; }
    }
}
