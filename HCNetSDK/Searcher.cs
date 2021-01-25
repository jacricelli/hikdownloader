namespace HikDownloader.HCNetSDK
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Buscador.
    /// </summary>
    public static class Searcher
    {
        /// <summary>
        /// Comienzo del proceso de búsqueda.
        /// </summary>
        public static event EventHandler OnStart;

        /// <summary>
        /// Fin del proceso de búsqueda.
        /// </summary>
        public static event EventHandler OnFinish;

        /// <summary>
        /// Comienzo de búsqueda en un canal.
        /// </summary>
        public static event EventHandler OnBegin;

        /// <summary>
        /// Fin de la búsqueda en un canal.
        /// </summary>
        public static event EventHandler OnEnd;

        /// <summary>
        /// Resultado de la búsqueda.
        /// </summary>
        public static event EventHandler OnResult;

        /// <summary>
        /// Error en la búsqueda.
        /// </summary>
        public static event EventHandler OnError;

        /// <summary>
        /// Cancelación de la búsqueda.
        /// </summary>
        public static event EventHandler OnCancel;

        /// <summary>
        /// Manejador de la búsqueda.
        /// </summary>
        private static int handle = -1;

        /// <summary>
        /// Indica que la búsqueda ha sido cancelada.
        /// </summary>
        private static bool cancelled = false;

        /// <summary>
        /// Obtiene un valor que indica si la búsqueda se está llevando a cabo.
        /// </summary>
        public static bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Realiza una búsqueda.
        /// </summary>
        /// <param name="channels">Uno o más canales.</param>
        /// <param name="start">Comienzo.</param>
        /// <param name="end">Finalización.</param>
        public static void Search(Channel[] channels, DateTime start, DateTime end)
        {
            var skipCancelEvent = false;

            OnStart?.Invoke(null, new EventArgs());

            foreach (var channel in channels)
            {
                if (cancelled)
                {
                    if (!skipCancelEvent)
                    {
                        OnCancel?.Invoke(null, new SearchEvent(channel));
                    }

                    break;
                }

                OnBegin?.Invoke(null, new SearchEvent(channel));

                IsRunning = true;
                cancelled = false;

                foreach (var from in EachDay(start, end))
                {
                    var thru = from.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (thru > end)
                    {
                        thru = end;
                    }

                    var conditions = default(NET_DVR_FILECOND_V40);
                    conditions.lChannel = channel.Number;
                    conditions.dwFileType = 0xff;
                    conditions.dwIsLocked = 0xff;

                    conditions.struStartTime.dwYear = (uint)from.Year;
                    conditions.struStartTime.dwMonth = (uint)from.Month;
                    conditions.struStartTime.dwDay = (uint)from.Day;
                    conditions.struStartTime.dwHour = (uint)from.Hour;
                    conditions.struStartTime.dwMinute = (uint)from.Minute;
                    conditions.struStartTime.dwSecond = (uint)from.Second;

                    conditions.struStopTime.dwYear = (uint)thru.Year;
                    conditions.struStopTime.dwMonth = (uint)thru.Month;
                    conditions.struStopTime.dwDay = (uint)thru.Day;
                    conditions.struStopTime.dwHour = (uint)thru.Hour;
                    conditions.struStopTime.dwMinute = (uint)thru.Minute;
                    conditions.struStopTime.dwSecond = (uint)thru.Second;

                    handle = NET_DVR_FindFile_V40(Session.User.Identifier, ref conditions);
                    if (handle > -1)
                    {
                        var record = default(NET_DVR_FINDDATA_V30);
                        while (true)
                        {
                            if (cancelled)
                            {
                                skipCancelEvent = true;

                                OnCancel?.Invoke(null, new SearchEvent(channel));

                                break;
                            }

                            var result = NET_DVR_FindNextFile_V30(handle, ref record);
                            if (result == NET_DVR_ISFINDING)
                            {
                                continue;
                            }
                            else if (result == NET_DVR_FILE_SUCCESS)
                            {
                                var videoStart = string.Format(
                                    "{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                                    record.struStartTime.dwYear,
                                    record.struStartTime.dwMonth,
                                    record.struStartTime.dwDay,
                                    record.struStartTime.dwHour,
                                    record.struStartTime.dwMinute,
                                    record.struStartTime.dwSecond
                                    );
                                var videoEnd = string.Format(
                                    "{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                                    record.struStopTime.dwYear,
                                    record.struStopTime.dwMonth,
                                    record.struStopTime.dwDay,
                                    record.struStopTime.dwHour,
                                    record.struStopTime.dwMinute,
                                    record.struStopTime.dwSecond
                                    );
                                var recording = new Recording(
                                    channel,
                                    new Video(record.sFileName, record.dwFileSize, DateTime.Parse(videoStart), DateTime.Parse(videoEnd)));

                                OnResult?.Invoke(null, new SearchResult(recording));
                            }
                            else if (result == NET_DVR_FIND_TIMEOUT)
                            {
                                OnError?.Invoke(null, new SearchError(channel, NET_DVR_FIND_TIMEOUT));

                                break;
                            }
                            else if (result == NET_DVR_FILE_NOFIND || result == NET_DVR_NOMOREFILE)
                            {
                                break;
                            }
                        }

                        NET_DVR_FindClose_V30(handle);
                        handle = -1;
                    }
                    else
                    {
                        OnError?.Invoke(null, new SearchError(channel, SDK.GetLastError()));
                    }
                }

                OnEnd?.Invoke(null, new SearchEvent(channel));
            }

            IsRunning = false;
            cancelled = false;

            OnFinish?.Invoke(null, new EventArgs());
        }

        /// <summary>
        /// Cancela la búsqueda.
        /// </summary>
        public static void Cancel()
        {
            if (IsRunning && !cancelled)
            {
                cancelled = true;
            }
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
        private struct NET_DVR_FILECOND_V40
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
        private struct NET_DVR_FINDDATA_V30
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
        private struct NET_DVR_TIME
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
        private struct NET_DVR_SPECIAL_FINDINFO_UNION
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
    }
}
