namespace HikDownloader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implementación de HCNetSDK.
    /// </summary>
    public static class HCNetSDK
    {
        /// <summary>
        /// La descripción de los errores de HCNetSDK.
        /// </summary>
        private static Dictionary<uint, string> _errors = new Dictionary<uint, string>();

        /// <summary>
        /// Obtiene un valor que indica si se ha inicializado el entorno de programación.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        static HCNetSDK()
        {
            var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("HikDownloader.Resources.HCNetSDKErrors.txt");
            using (var reader = new StreamReader(resource))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var error = line.Split('\t');
                    _errors.Add(uint.Parse(error[0]), error[1]);
                }
            }
        }

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
        /// Obtiene el código de error generado por la última operación.
        /// </summary>
        /// <returns>El código de error de la última operación.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_GetLastError")]
        public static extern uint GetLastErrno();

        /// <summary>
        /// Obtiene la descripción del error generado por la última operación.
        /// </summary>
        /// <returns>La descripción del error de la última operación.</returns>
        public static string GetLastError()
        {
            var errno = GetLastErrno();
            if (_errors.ContainsKey(errno))
            {
                return _errors[errno];
            }

            return $"El código de error '{errno}' no se encuentra documentado.";
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
