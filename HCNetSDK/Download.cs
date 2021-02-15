namespace HikDownloader.HCNetSDK
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Descarga.
    /// </summary>
    public static class Download
    {
        /// <summary>
        /// Prepara la descarga de un archivo.
        /// </summary>
        /// <param name="lUserID">Identificador del usuario.</param>
        /// <param name="sDVRFileName">Nombre del archivo.</param>
        /// <param name="sSavedFileName">Ruta de acceso donde se guardará el archivo.</param>
        /// <returns>Devuelve -1 en caso de error, un valor mayor a cero como identificador de la descarga.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_GetFileByName")]
        public static extern int GetFileByName(int lUserID, string sDVRFileName, string sSavedFileName);

        /// <summary>
        /// Detiene la descarga de un archivo.
        /// </summary>
        /// <param name="lFileHandle">Identificador de la descarga.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_StopGetFile")]
        public static extern bool StopGetFile(int lFileHandle);

        /// <summary>
        /// Obtiene el progreso de una descarga.
        /// </summary>
        /// <param name="lFileHandle">Identificador de la descarga.</param>
        /// <returns>Progreso de la descarga.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_GetDownloadPos")]
        public static extern int GetDownloadPos(int lFileHandle);

        /// <summary>
        /// Controla la reproducción o descarga de grabaciones.
        /// </summary>
        /// <param name="lPlayHandle">Identificador de la reproducción o descarga.</param>
        /// <param name="dwControlCode">Código de comando para la reproducción o descarga.</param>
        /// <param name="lpInBuffer">Puntero de parámetros de entrada.</param>
        /// <param name="dwInValue">Tamaño del parámetro de entrada.</param>
        /// <param name="lpOutBuffer">Puntero de parámetros de salida.</param>
        /// <param name="lpOutValue">Tamaño del parámetro de salida.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_PlayBackControl_V40")]
        public static extern bool PlayBackControl(int lPlayHandle, uint dwControlCode, IntPtr lpInBuffer, uint dwInValue, IntPtr lpOutBuffer, ref uint lpOutValue);
    }
}
