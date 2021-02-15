namespace HikDownloader.HCNetSDK
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Sesión.
    /// </summary>
    public static class Session
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public static int UserId { get; private set; } = -1;

        /// <summary>
        /// Inicia una sesión.
        /// </summary>
        /// <param name="config">Configuración.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Login(Config.HCNetSDKSessionConfig config)
        {
            if (UserId < 0)
            {
                var deviceInfo = default(NET_DVR_DEVICEINFO);
                UserId = NET_DVR_Login_V30(config.Address, config.Port, config.UserName, config.Password, ref deviceInfo);

                return UserId > -1;
            }

            return false;
        }

        /// <summary>
        /// Cierra una sesión.
        /// </summary>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        public static bool Logout()
        {
            if (UserId > -1 && NET_DVR_Logout(UserId))
            {
                UserId = -1;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Información del dispositivo.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        private struct NET_DVR_DEVICEINFO
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
        private static extern int NET_DVR_Login_V30(string sDVRIP, int wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO lpDeviceInfo);

        /// <summary>
        /// Cierra una sesión.
        /// </summary>
        /// <param name="iUserID">Identificador del usuario.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_Logout(int iUserID);
    }
}
