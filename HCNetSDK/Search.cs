namespace HikDownloader.HCNetSDK
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Búsqueda.
    /// </summary>
    public static class Search
    {
        /// <summary>
        /// Longitud para el dato GUID.
        /// </summary>
        public const int GUID_LEN = 16;

        /// <summary>
        /// Longitud del número de tarjeta para exterior.
        /// </summary>
        public const int CARDNUM_LEN_OUT = 32;

        /// <summary>
        /// Búsqueda completada.
        /// </summary>
        public const int NET_DVR_FILE_SUCCESS = 1000;

        /// <summary>
        /// No se ha encontrado el archivo.
        /// </summary>
        public const int NET_DVR_FILE_NOFIND = 1001;

        /// <summary>
        /// Búsqueda en progreso.
        /// </summary>
        public const int NET_DVR_ISFINDING = 1002;

        /// <summary>
        /// No hay más archivos.
        /// </summary>
        public const int NET_DVR_NOMOREFILE = 1003;

        /// <summary>
        /// Error en la búsqueda.
        /// </summary>
        public const int NET_DVR_FILE_EXCEPTION = 1004;

        /// <summary>
        /// Tiempo de espera agotado.
        /// </summary>
        public const int NET_DVR_FIND_TIMEOUT = 10005;

        /// <summary>
        /// Condiciones de búsqueda.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_FILECOND
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
        public struct NET_DVR_FINDDATA
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
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_FindFile_V40")]
        public static extern int FindFile(int lUserID, ref NET_DVR_FILECOND pFindCond);

        /// <summary>
        /// Obtiene el próximo archivo de la búsqueda.
        /// </summary>
        /// <param name="lFindHandle">Identificador de la búsqueda.</param>
        /// <param name="lpFindData">Datos de la búsqueda.</param>
        /// <returns>Devuelve -1 en caso de error, otro valor que indica el estado de la búsqueda.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_FindNextFile_V30")]
        public static extern int FindNextFile(int lFindHandle, ref NET_DVR_FINDDATA lpFindData);

        /// <summary>
        /// Detiene una búsqueda.
        /// </summary>
        /// <param name="lFindHandle">Identificador de la búsqueda.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_FindClose_V30")]
        public static extern bool FindClose(int lFindHandle);
    }
}
