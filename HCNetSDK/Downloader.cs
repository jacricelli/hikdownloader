namespace HikDownloader.HCNetSDK
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Timers;

    /// <summary>
    /// Descarga de archivos.
    /// </summary>
    public static class Downloader
    {
        /// <summary>
        /// Directorio de descarga.
        /// </summary>
        public static string DownloadDir { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que especifica la cantidad de descargas en paralelo permitidas.
        /// </summary>
        public static int ParallelDownloads { get; set; } = 3;

        /// <summary>
        /// Obtiene un valor que indica si se están descargando grabaciones.
        /// </summary>
        public static bool IsRunning { get; private set; } = false;

        /// <summary>
        /// Grabaciones.
        /// </summary>
        private static List<Recording> recordings;

        /// <summary>
        /// Descargas.
        /// </summary>
        private static Dictionary<Recording, int> downloads;

        /// <summary>
        /// Gestor de las descargas.
        /// </summary>
        private static Timer downloadManager;

        /// <summary>
        /// Indica que la descarga se ha cancelado.
        /// </summary>
        private static bool cancelled;

        /// <summary>
        /// Comprueba si existe el archivo de una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <returns>Devuelve TRUE si el archivo existe, FALSE de lo contrario.</returns>
        public static bool RecordingFileExists(Recording recording)
        {
            return File.Exists(GetRecordingPath(recording, "avi"));
        }

        public static void Download(List<Recording> recordings)
        {
            if (IsRunning)
            {
                return;
            }

            Downloader.recordings = recordings;

            downloads = new Dictionary<Recording, int>();
            downloadManager = new Timer(1000);
            downloadManager.Elapsed += DownloadManager_Tick;
            downloadManager.AutoReset = true;
            downloadManager.Start();

            IsRunning = true;
            cancelled = false;
        }

        public static void Cancel()
        {
            if (IsRunning)
            {
                downloadManager.Stop();
                downloadManager.Dispose();

                cancelled = true;
                IsRunning = false;
            }
        }

        private static void DownloadManager_Tick(Object source, ElapsedEventArgs e)
        {
            if (recordings.Count == 0 && downloads.Count == 0)
            {
                Cancel();

                return;
            }

            var slots = ParallelDownloads - downloads.Count;
            if (slots > recordings.Count)
            {
                slots = recordings.Count;
            }

            for (var slot = 1; slot <= slots; slot++)
            {
                PrepareDownload(recordings.First<Recording>());
            }

            var downloadsCopy = new Dictionary<Recording, int>(downloads);
            foreach (KeyValuePair<Recording, int> entry in downloadsCopy)
            {
                var progress = NET_DVR_GetDownloadPos(entry.Value);
                if (progress < 0)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " Error progress = 0");
                }
                else if (progress >= 0 && progress < 100)
                {
                    System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " downloaded " + progress + "%");
                }
                else if (progress == 100)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    File.Move(GetRecordingPath(entry.Key), GetRecordingPath(entry.Key, "avi"));

                    System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " done!");
                }
                else if (progress == 200)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " network error!");
                }
            }

            System.Diagnostics.Debug.WriteLine(recordings.Count);
        }

        private static void PrepareDownload(Recording recording)
        {
            recordings.Remove(recording);

            if (RecordingFileExists(recording))
            {
                return;
            }

            var file = GetRecordingPath(recording);
            var directory = Path.GetDirectoryName(file);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var handle = NET_DVR_GetFileByName(Session.User.Identifier, recording.Video.FileName, file);
            if (handle > -1)
            {
                uint iOutValue = 0;
                if (NET_DVR_PlayBackControl_V40(handle, 1, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    downloads.Add(recording, handle);
                }
            }

            // Error!
        }









        //
        //public static bool IsRunning { get; private set; }

        //private static bool cancelled = false;

        //private static Dictionary<string, Recording> downloads;

        //private static Dictionary<Recording, int> curDownloads;

        //private static void xDownloads_Tick(Object source, ElapsedEventArgs e)
        //{
        //    if (cancelled)
        //    {
        //        return;
        //    }

        //    var maxDownloads = 3;
        //    var total = curDownloads.Count;
        //    var availableSlots = maxDownloads - total;

        //    for (var i = 1; i <= availableSlots; i++)
        //    {
        //        var x = downloads.Keys.First();
        //        var z = downloads[x];
        //        downloads.Remove(x);

        //        var handle = PrepareDownload(z);
        //        if (handle > -1)
        //        {
        //            curDownloads.Add(z, handle);
        //        }
        //    }

        //    var copy = new Dictionary<Recording, int>(curDownloads);
            //foreach (KeyValuePair<Recording, int> entry in copy)
            //{
            //    var progress = Downloader.GetDownloadProgress(entry.Value);
            //    if (progress < 0)
            //    {
            //        StopDownload(entry.Value);
            //        curDownloads.Remove(entry.Key);

            //        System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " Error progress = 0");
            //    }
            //    else if (progress >= 0 && progress < 100)
            //    {
            //        System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " downloaded " + progress + "%");
            //    }
            //    else if (progress == 100)
            //    {
            //        StopDownload(entry.Value);
            //        curDownloads.Remove(entry.Key);

            //        System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " done!");
            //    }
            //    else if (progress == 200)
            //    {
            //        StopDownload(entry.Value);
            //        curDownloads.Remove(entry.Key);

            //        System.Diagnostics.Debug.WriteLine(entry.Key.Video.FileName + " network error!");
            //    }
            //}

        //    System.Diagnostics.Debug.WriteLine(downloads.Count());
        //}

        //private static int PrepareDownload(Recording recording)
        //{
        //    var file = GetRecordingPath(recording, "avi");
        //    var directory = Path.GetDirectoryName(file);



        //    var handle = Downloader;
        //    if (handle > -1)
        //    {
        //        if (StartDownload(handle))
        //        {
        //            return handle;
        //        }
        //    }

        //    return -1;
        //}

        ///// <summary>
        ///// Comienza la descarga de un archivo.
        ///// </summary>
        ///// <param name="lFileHandle">Identificador de la descarga.</param>
        ///// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        //public static bool StartDownload(int lFileHandle)
        //{
        //    uint iOutValue = 0;

        //    return NET_DVR_PlayBackControl_V40(lFileHandle, 1, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue);
        //}

        //public static void Cancel()
        //{
        //    cancelled = true;
        //    downloadTimer.Stop();
        //    downloadTimer.Dispose();

        //    System.Diagnostics.Debug.WriteLine("Cancel");
        //}

        /// <summary>
        /// Obtiene la ruta absoluta para el archivo de una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="extension">Extensión del archivo.</param>
        /// <returns>Ruta al archivo.</returns>
        private static string GetRecordingPath(Recording recording, string extension = "")
        {
            return string.Format(
                "{0}\\{1}-{2:00}\\Channel {3:00}\\{4}_{5}_{6}{7}",
                DownloadDir,
                recording.Video.Start.Year,
                recording.Video.Start.Month,
                recording.Channel.Number,
                recording.Video.Start.ToString("yyyy-MM-dd"),
                recording.Video.Start.ToString("HHmmss"),
                recording.Video.End.ToString("HHmmss"),
                extension != string.Empty ? $".{extension}" : "");
        }

        /// <summary>
        /// Prepara la descarga de un archivo.
        /// </summary>
        /// <param name="lUserID">Identificador del usuario.</param>
        /// <param name="sDVRFileName">Nombre del archivo.</param>
        /// <param name="sSavedFileName">Ruta de acceso donde se guardará el archivo.</param>
        /// <returns>Devuelve -1 en caso de error, un valor mayor a cero como identificador de la descarga.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_GetFileByName(int lUserID, string sDVRFileName, string sSavedFileName);

        /// <summary>
        /// Detiene la descarga de un archivo.
        /// </summary>
        /// <param name="lFileHandle">Identificador de la descarga.</param>
        /// <returns>Devuelve TRUE si la operación se ha completado exitosamente, FALSE de lo contrario.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_StopGetFile(int lFileHandle);

        /// <summary>
        /// Obtiene el progreso de una descarga.
        /// </summary>
        /// <param name="lFileHandle">Identificador de la descarga.</param>
        /// <returns>Progreso de la descarga.</returns>
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern int NET_DVR_GetDownloadPos(int lFileHandle);

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
        [DllImport(@"lib\HCNetSDK.dll")]
        private static extern bool NET_DVR_PlayBackControl_V40(int lPlayHandle, uint dwControlCode, IntPtr lpInBuffer, uint dwInValue, IntPtr lpOutBuffer, ref uint lpOutValue);
    }
}
