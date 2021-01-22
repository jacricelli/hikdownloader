namespace HikDownloader
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Implementación de HCNetSDK.
    /// </summary>
    public static class HCNetSDK
    {
        /// <summary>
        /// Indica si se ha inicializado el entorno de programación.
        /// </summary>
        private static bool initialized = false;

        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        private static int userId = -1;

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
        /// Inicializa el entorno de programación.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Initialize()
        {
            initialized = NET_DVR_Init();

            return initialized;
        }

        /// <summary>
        /// Libera recursos utilizados por HCNetSDK.dll.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Cleanup()
        {
            if (initialized)
            {
                initialized = !NET_DVR_Cleanup();
            }

            return initialized == false;
        }

        /// <summary>
        /// Obtiene el código de error generado por la última operación.
        /// </summary>
        /// <returns>El código de error de la última operación.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_GetLastError")]
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
        /// Inicia una sesión.
        /// </summary>
        /// <param name="address">Host o dirección IP.</param>
        /// <param name="port">Puerto.</param>
        /// <param name="userName">Nombre de usuario.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Login(string address, int port, string userName, string password)
        {
            if (!IsLoggedIn)
            {
                var lpDeviceInfo = default(NET_DVR_DEVICEINFO_V30);
                userId = NET_DVR_Login_V30(address, port, userName, password, ref lpDeviceInfo);
            }

            return IsLoggedIn;
        }

        /// <summary>
        /// Cierra la sesión activa.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Logout()
        {
            if (IsLoggedIn)
            {
                if (NET_DVR_Logout(userId))
                {
                    userId = -1;
                }
            }

            return !IsLoggedIn;
        }

        /// <summary>
        /// Obtiene un valor que indica si se ha iniciado sesión.
        /// </summary>
        public static bool IsLoggedIn => userId > -1;

        /// <summary>
        /// Obtiene el identificador del usuario.
        /// </summary>
        public static int UserId => userId;

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

        /// <summary>
        /// Crea una sesión.
        /// </summary>
        /// <param name="sDVRIP">Host o dirección de IP del dispositivo.</param>
        /// <param name="wDVRPort">Puerto.</param>
        /// <param name="sUserName">Nombre de usuario.</param>
        /// <param name="sPassword">Contraseña.</param>
        /// <param name="lpDeviceInfo">Información del dispositivo.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_Login_V30(string sDVRIP, int wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);

        /// <summary>
        /// Cierra una sesión.
        /// </summary>
        /// <param name="iUserID">Identificador del usuario.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_Logout(int iUserID);
    }
}
