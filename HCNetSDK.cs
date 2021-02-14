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
        #region Inicialización y limpieza
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
        #endregion

        #region Registro de eventos
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
        /// Controla el registro de mensajes.
        /// </summary>
        /// <param name="bLogEnable">Nivel de registro (0 para deshabilitar, 1 para errores, 2 para errores y depuración, 3 para todos los mensajes).</param>
        /// <param name="strLogDir">Ruta de acceso al directorio donde se guardan los archivos.</param>
        /// <param name="bAutoDel">Indica si se habilita la eliminación automática de archivos de registro cuando se alcance el número máximo.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_SetLogToFile(int bLogEnable, string strLogDir, bool bAutoDel);
        #endregion

        #region Sesión
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        private static int _userId = -1;

        /// <summary>
        /// Inicia una sesión.
        /// </summary>
        /// <param name="config">Configuración.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Login(HCNetSDKDeviceConfig config)
        {
            if (_userId < 0)
            {
                var deviceInfo = default(NET_DVR_DEVICEINFO_V30);
                _userId = NET_DVR_Login_V30(config.Address, config.Port, config.UserName, config.Password, ref deviceInfo);

                return _userId > -1;
            }

            return false;
        }

        /// <summary>
        /// Cierra una sesión.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Logout()
        {
            if (_userId > -1 && NET_DVR_Logout(_userId))
            {
                _userId = -1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Información del dispositivo.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct NET_DVR_DEVICEINFO_V30
        {
            /// <summary>
            /// Número de serie.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;

            /// <summary>
            /// Número de entradas de alarma analógicas.
            /// </summary>
            public byte byAlarmInPortNum;

            /// <summary>
            /// Número de salidas de alarma analógicas.
            /// </summary>
            public byte byAlarmOutPortNum;

            /// <summary>
            /// Cantidad de HDDs.
            /// </summary>
            public byte byDiskNum;

            /// <summary>
            /// Tipo de dispositivo.
            /// </summary>
            public byte byDVRType;

            /// <summary>
            /// Total de canales analógicos.
            /// </summary>
            public byte byChanNum;

            /// <summary>
            /// Número del canal analógico inicial.
            /// </summary>
            public byte byStartChan;

            /// <summary>
            /// Total de canales de audio bidireccionales.
            /// </summary>
            public byte byAudioChanNum;

            /// <summary>
            /// Total de canales digitales (low 8-bit).
            /// </summary>
            public byte byIPChanNum;

            /// <summary>
            /// Número del canal cero.
            /// </summary>
            public byte byZeroChanNum;

            /// <summary>
            /// Tipo de protocolo de transmisión de flujo principal.
            /// </summary>
            public byte byMainProto;

            /// <summary>
            /// Tipo de protocolo de transmisión de subflujo.
            /// </summary>
            public byte bySubProto;

            /// <summary>
            /// Capacidades.
            /// </summary>
            public byte bySupport;

            /// <summary>
            /// Capacidades extendidas.
            /// </summary>
            public byte bySupport1;

            /// <summary>
            /// Capacidades extendidas.
            /// </summary>
            public byte bySupport2;

            /// <summary>
            /// Modelo del dispositivo.
            /// </summary>
            public ushort wDevType;

            /// <summary>
            /// Capacidades extendidas.
            /// </summary>
            public byte bySupport3;

            /// <summary>
            /// Compatibilidad con múltiples flujos.
            /// </summary>
            public byte byMultiStreamProto;

            /// <summary>
            /// Número del canal digital inicial.
            /// </summary>
            public byte byStartDChan;

            /// <summary>
            /// Número del canal de audio bidireccionales inicial.
            /// </summary>
            public byte byStartDTalkChan;

            /// <summary>
            /// Total de canales digitales (high 8-bit).
            /// </summary>
            public byte byHighDChanNum;

            /// <summary>
            /// Capacidades extendidas.
            /// </summary>
            public byte bySupport4;

            /// <summary>
            /// Tipos de idiomas admitidos.
            /// </summary>
            public byte byLanguageType;

            /// <summary>
            /// Reservado.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /// <summary>
        /// Crea una sesión.
        /// </summary>
        /// <param name="sDVRIP">Dirección.</param>
        /// <param name="wDVRPort">Puerto.</param>
        /// <param name="sUserName">Nombre de usuario.</param>
        /// <param name="sPassword">Contraseña.</param>
        /// <param name="lpDeviceInfo">Información del dispositivo.</param>
        /// <returns>Devuelve -1 en caso que el usuario y/o contraseña sean incorrectos, un valor mayor a cero como identificador del usuario</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_Login_V30(string sDVRIP, int wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);

        /// <summary>
        /// Cierra una sesión.
        /// </summary>
        /// <param name="iUserID">Identificador del usuario.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_Logout(int iUserID);
        #endregion

        #region Errores
        /// <summary>
        /// La descripción de los errores de HCNetSDK.
        /// </summary>
        private static Dictionary<uint, string> _errors = new Dictionary<uint, string>();

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
        #endregion
    }
}
