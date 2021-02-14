namespace HikDownloader
{
    using System;
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

        #region Búsqueda
        /// <summary>
        /// Realiza una búsqueda.
        /// </summary>
        /// <param name="channels">Uno o más canales.</param>
        /// <param name="start">Fecha de comienzo.</param>
        /// <param name="end">Fecha de finalización.</param>
        /// <returns>Resultados de la búsqueda.</returns>
        public static List<NET_DVR_FINDDATA_V30> Search(int[] channels, DateTime start, DateTime end)
        {
            var results = new List<NET_DVR_FINDDATA_V30>();

            foreach (var channel in channels)
            {
                foreach (var from in EachDay(start, end))
                {
                    var thru = from.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (thru > end)
                    {
                        thru = end;
                    }

                    var conditions = default(NET_DVR_FILECOND_V40);
                    conditions.lChannel = channel;
                    conditions.dwFileType = 0xff;
                    conditions.dwIsLocked = 0xff;

                    conditions.struStartTime.dwYear = (uint)from.Year;
                    conditions.struStartTime.dwMonth = (uint)from.Month;
                    conditions.struStartTime.dwDay = (uint)from.Day;
                    conditions.struStartTime.dwHour = 0;
                    conditions.struStartTime.dwMinute = 0;
                    conditions.struStartTime.dwSecond = 0;

                    conditions.struStopTime.dwYear = (uint)thru.Year;
                    conditions.struStopTime.dwMonth = (uint)thru.Month;
                    conditions.struStopTime.dwDay = (uint)thru.Day;
                    conditions.struStopTime.dwHour = 23;
                    conditions.struStopTime.dwMinute = 59;
                    conditions.struStopTime.dwSecond = 59;

                    var handle = NET_DVR_FindFile_V40(_userId, ref conditions);
                    if (handle > -1)
                    {
                        var record = default(NET_DVR_FINDDATA_V30);
                        while (true)
                        {
                            var result = NET_DVR_FindNextFile_V30(handle, ref record);
                            if (result == NET_DVR_ISFINDING)
                            {
                                continue;
                            }
                            else if (result == NET_DVR_FILE_SUCCESS)
                            {
                                if (!results.Contains(record))
                                {
                                    results.Add(record);
                                }
                            }
                            else if (result == NET_DVR_FIND_TIMEOUT || result == NET_DVR_FILE_NOFIND || result == NET_DVR_NOMOREFILE || result == NET_DVR_FILE_EXCEPTION)
                            {
                                break;
                            }
                        }

                        NET_DVR_FindClose_V30(handle);
                    }
                    else
                    {
                        throw new Exception(GetLastError());
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Obtiene cada día entre dos fechas.
        /// </summary>
        /// <param name="from">Desde.</param>
        /// <param name="thru">Hasta</param>
        /// <returns>Cada día entre <paramref name="from"/> y <paramref name="thru"/> inclusive.</returns>
        private static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        /// <summary>
        /// Longitud para el dato GUID.
        /// </summary>
        private const int GUID_LEN = 16;

        /// <summary>
        /// Longitud del número de tarjeta para exterior.
        /// </summary>
        private const int CARDNUM_LEN_OUT = 32;

        /// <summary>
        /// Búsqueda completada.
        /// </summary>
        private const int NET_DVR_FILE_SUCCESS = 1000;

        /// <summary>
        /// No se ha encontrado el archivo.
        /// </summary>
        private const int NET_DVR_FILE_NOFIND = 1001;

        /// <summary>
        /// Búsqueda en progreso.
        /// </summary>
        private const int NET_DVR_ISFINDING = 1002;

        /// <summary>
        /// No hay más archivos.
        /// </summary>
        private const int NET_DVR_NOMOREFILE = 1003;

        /// <summary>
        /// Error en la búsqueda.
        /// </summary>
        private const int NET_DVR_FILE_EXCEPTION = 1004;

        /// <summary>
        /// Tiempo de espera agotado.
        /// </summary>
        private const int NET_DVR_FIND_TIMEOUT = 10005;

        /// <summary>
        /// Condiciones de búsqueda.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FILECOND_V40
        {
            /// <summary>
            /// Número de canal.
            /// </summary>
            public int lChannel;

            /// <summary>
            /// Tipo de archivo.
            /// </summary>
            public uint dwFileType;

            /// <summary>
            /// Indica si el archivo está bloqueado.
            /// </summary>
            public uint dwIsLocked;

            /// <summary>
            /// Indica si se utiliza número de tarjeta.
            /// </summary>
            public uint dwUseCardNo;

            /// <summary>
            /// Número de tarjeta.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = CARDNUM_LEN_OUT, ArraySubType = UnmanagedType.I1)]
            public byte[] sCardNumber;

            /// <summary>
            /// Fecha de comienzo.
            /// </summary>
            public NET_DVR_TIME struStartTime;

            /// <summary>
            /// Fecha de finalización.
            /// </summary>
            public NET_DVR_TIME struStopTime;

            /// <summary>
            /// Indica si se extrae el cuadro.
            /// </summary>
            public byte byDrawFrame;

            /// <summary>
            /// Tipo de búsqueda.
            /// </summary>
            public byte byFindType;

            /// <summary>
            /// Indica si se realiza una búsqueda rápida.
            /// </summary>
            public byte byQuickSearch;

            /// <summary>
            /// Indica si se realiza una búsqueda especial.
            /// </summary>
            public byte bySpecialFindInfoType;

            /// <summary>
            /// Número de volumen.
            /// </summary>
            public uint dwVolumeNum;

            /// <summary>
            /// GUID.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = GUID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byWorkingDeviceGUID;

            /// <summary>
            /// Unión de la condición de búsqueda.
            /// </summary>
            public NET_DVR_SPECIAL_FINDINFO_UNION uSpecialFindInfo;

            /// <summary>
            /// Reservado.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }

        /// <summary>
        /// Resultado de búsqueda.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_FINDDATA_V30
        {
            /// <summary>
            /// Nombre del archivo.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string sFileName;

            /// <summary>
            /// Fecha de comienzo.
            /// </summary>
            public NET_DVR_TIME struStartTime;

            /// <summary>
            /// Fecha de finalización.
            /// </summary>
            public NET_DVR_TIME struStopTime;

            /// <summary>
            /// Tamaño del archivo.
            /// </summary>
            public uint dwFileSize;

            /// <summary>
            /// Número de tarjeta.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCardNum;

            /// <summary>
            /// Indica si el archivo está bloqueado.
            /// </summary>
            public byte byLocked;

            /// <summary>
            /// Tipo de archivo.
            /// </summary>
            public byte byFileType;

            /// <summary>
            /// Reservado.
            /// </summary>
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        /// <summary>
        /// Estructura del parámetro de tiempo.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME
        {
            /// <summary>
            /// Año.
            /// </summary>
            public uint dwYear;

            /// <summary>
            /// Mes.
            /// </summary>
            public uint dwMonth;

            /// <summary>
            /// Día.
            /// </summary>
            public uint dwDay;

            /// <summary>
            /// Hora.
            /// </summary>
            public uint dwHour;

            /// <summary>
            /// Minuto.
            /// </summary>
            public uint dwMinute;

            /// <summary>
            /// Segundo.
            /// </summary>
            public uint dwSecond;
        }

        /// <summary>
        /// Especifica la unión de la condición de búsqueda.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Explicit)]
        public struct NET_DVR_SPECIAL_FINDINFO_UNION
        {
            /// <summary>
            /// Tamaño de la unión.
            /// </summary>
            [FieldOffsetAttribute(0)]
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byLenth;
        }

        /// <summary>
        /// Lleva a cabo una búsqueda de archivos.
        /// </summary>
        /// <param name="lUserID">Identificador del usuario.</param>
        /// <param name="pFindCond">Condiciones de búsqueda.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_FindFile_V40(int lUserID, ref NET_DVR_FILECOND_V40 pFindCond);

        /// <summary>
        /// Obtiene el próximo archivo de la búsqueda.
        /// </summary>
        /// <param name="lFindHandle">Identificador de la búsqueda.</param>
        /// <param name="lpFindData">Datos de la búsqueda.</param>
        /// <returns>Devuelve -1 en caso de error, otro valor que indica el estado de la búsqueda.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_FindNextFile_V30(int lFindHandle, ref NET_DVR_FINDDATA_V30 lpFindData);

        /// <summary>
        /// Detiene una búsqueda.
        /// </summary>
        /// <param name="lFindHandle">Identificador de la búsqueda.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_FindClose_V30(int lFindHandle);
        #endregion
    }
}
