namespace HikDownloader.HCNetSDK
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Registro de mensajes.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Controla el registro de mensajes.
        /// </summary>
        /// <param name="config">Configuración.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool SetLogToFile(Config.HCNetSDKLogConfig config)
        {
            return NET_DVR_SetLogToFile(config.Level, config.Dir, config.AutoDel);
        }

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
