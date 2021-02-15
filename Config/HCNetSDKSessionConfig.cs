namespace HikDownloader.Config
{
    /// <summary>
    /// Configuración de la sesión.
    /// </summary>
    public class HCNetSDKSessionConfig
    {
        /// <summary>
        /// Dirección.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Puerto.
        /// </summary>
        public int Port { get; set; } = 8000;

        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Contraseña.
        /// </summary>
        public string Password { get; set; }
    }
}
