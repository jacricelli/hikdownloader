namespace HikDownloader.HCNetSDK
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implementación de HCNetSDK.
    /// </summary>
    public static class SDK
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
        /// Obtiene el código de error generado por la última operación.
        /// </summary>
        /// <returns>El código de error de la última operación.</returns>
        [DllImport(@"tools\HCNetSDK\HCNetSDK.dll", EntryPoint = "NET_DVR_GetLastError")]
        public static extern uint GetLastError();

        /// <summary>
        /// Habilita el registro de mensajes.
        /// </summary>
        /// <param name="path">Ruta de acceso donde se guardan los archivos.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool EnableLogging(string path)
        {
            return NET_DVR_SetLogToFile(3, path, true);
        }

        /// <summary>
        /// Inicializa el entorno de programación.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"tools\HCNetSDK\HCNetSDK.dll")]
        private static extern bool NET_DVR_Init();

        /// <summary>
        /// Libera recursos utilizados por HCNetSDK.dll.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"tools\HCNetSDK\HCNetSDK.dll")]
        private static extern bool NET_DVR_Cleanup();

        /// <summary>
        /// Controla el registro de mensajes.
        /// </summary>
        /// <param name="bLogEnable">Nivel de registro (0 para deshabilitar, 1 para errores, 2 para errores y depuración, 3 para todos los mensajes).</param>
        /// <param name="strLogDir">Ruta de acceso al directorio donde se guardan los archivos.</param>
        /// <param name="bAutoDel">Indica si se habilita la eliminación automática de archivos de registro cuando se alcance el número máximo.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"tools\HCNetSDK\HCNetSDK.dll")]
        private static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);
    }
}