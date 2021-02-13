namespace HikDownloader
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implementación de HCNetSDK.
    /// </summary>
    public static class HCNetSDK
    {
        /// <summary>
        /// Obtiene un valor que indica si se ha inicializado el entorno de programación.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Inicializa el entorno de programación.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Initialize()
        {
            IsInitialized = NET_DVR_Init();

            return IsInitialized;
        }

        /// <summary>
        /// Libera recursos utilizados por HCNetSDK.dll.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Cleanup()
        {
            if (IsInitialized)
            {
                IsInitialized = !NET_DVR_Cleanup();
            }

            return IsInitialized == false;
        }

        /// <summary>
        /// Controla el registro de mensajes.
        /// </summary>
        /// <param name="config">Configuración.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool SetLogToFile(HCNetSDKLogConfig config)
        {
            return NET_DVR_SetLogToFile(config.Level, config.Dir, config.AutoDel);
        }

        /// <summary>
        /// Inicializa el entorno de programación.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_Init();

        /// <summary>
        /// Libera recursos utilizados por HCNetSDK.dll.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_Cleanup();

        /// <summary>
        /// Controla el registro de mensajes.
        /// </summary>
        /// <param name="bLogEnable">Nivel de registro (0 para deshabilitar, 1 para errores, 2 para errores y depuración, 3 para todos los mensajes).</param>
        /// <param name="strLogDir">Ruta de acceso al directorio donde se guardan los archivos.</param>
        /// <param name="bAutoDel">Indica si se habilita la eliminación automática de archivos de registro cuando se alcance el número máximo.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);
    }
}
