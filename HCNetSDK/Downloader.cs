namespace HikDownloader.HCNetSDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Timers;

    /// <summary>
    /// Descarga de archivos.
    /// </summary>
    public static class Downloader
    {
        /// <summary>
        /// Comienzo del proceso de descarga.
        /// </summary>
        public static event EventHandler OnStart;

        /// <summary>
        /// Fin del proceso de descarga.
        /// </summary>
        public static event EventHandler OnFinish;

        /// <summary>
        /// Comienzo de la descarga de una grabación.
        /// </summary>
        public static event EventHandler OnBegin;

        /// <summary>
        /// Fin de la descarga de una grabación.
        /// </summary>
        public static event EventHandler OnEnd;

        /// <summary>
        /// Descarga en progreso.
        /// </summary>
        public static event EventHandler OnDownloading;

        /// <summary>
        /// Error en la descarga.
        /// </summary>
        public static event EventHandler OnError;

        /// <summary>
        /// Cancelación de la descarga.
        /// </summary>
        public static event EventHandler OnCancel;

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
        /// Indica que la descarga se ha cancelado.
        /// </summary>
        private static bool cancelled;

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
        /// Comprueba si existe el archivo de una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <returns>Devuelve TRUE si el archivo existe, FALSE de lo contrario.</returns>
        public static bool RecordingFileExists(Recording recording)
        {
            return File.Exists(GetRecordingPath(recording, "avi"));
        }

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="recordings"></param>
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

            OnStart?.Invoke(null, new EventArgs());
        }

        /// <summary>
        /// Cancela las descargas.
        /// </summary>
        public static void Cancel()
        {
            if (IsRunning)
            {
                StopDownload();
            }
        }

        /// <summary>
        /// Detiene las descargas.
        /// </summary>
        private static void StopDownload()
        {
            downloadManager.Stop();
            downloadManager.Dispose();

            IsRunning = false;
            cancelled = true;

            if (downloads.Count > 0)
            {
                var copy = new Dictionary<Recording, int>(downloads);
                foreach (KeyValuePair<Recording, int> entry in copy)
                {
                    NET_DVR_StopGetFile(entry.Value);

                    downloads.Remove(entry.Key);

                    OnCancel?.Invoke(null, new DownloadEvent(entry.Key));
                }
            }

            OnFinish?.Invoke(null, new EventArgs());
        }

        /// <summary>
        /// Gestiona las descargas.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void DownloadManager_Tick(Object source, ElapsedEventArgs e)
        {
            if (cancelled)
            {
                return;
            }

            if (recordings.Count == 0 && downloads.Count == 0)
            {
                StopDownload();

                return;
            }

            var availableSlots = ParallelDownloads - downloads.Count;
            if (availableSlots > recordings.Count)
            {
                availableSlots = recordings.Count;
            }

            for (var i = 1; i <= availableSlots; i++)
            {
                var recording = recordings.First<Recording>();
                recordings.Remove(recording);

                if (RecordingFileExists(recording))
                {
                    OnEnd?.Invoke(null, new DownloadEvent(recording));

                    continue;
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

                        OnBegin?.Invoke(null, new DownloadEvent(recording));

                        continue;
                    }
                }

                OnError?.Invoke(null, new DownloadError(recording, SDK.GetLastError()));
            }

            var copy = new Dictionary<Recording, int>(downloads);
            foreach (KeyValuePair<Recording, int> entry in copy)
            {
                var progress = NET_DVR_GetDownloadPos(entry.Value);
                if (progress < 0)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    OnError?.Invoke(null, new DownloadError(entry.Key, SDK.GetLastError()));
                }
                else if (progress >= 0 && progress < 100)
                {
                    OnDownloading?.Invoke(null, new DownloadProgress(entry.Key, progress));
                }
                else if (progress == 100)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    File.Move(GetRecordingPath(entry.Key), GetRecordingPath(entry.Key, "avi"));

                    OnEnd?.Invoke(null, new DownloadEvent(entry.Key));
                }
                else if (progress == 200)
                {
                    NET_DVR_StopGetFile(entry.Value);
                    downloads.Remove(entry.Key);

                    OnError?.Invoke(null, new DownloadError(entry.Key, SDK.GetLastError()));
                }
            }
        }

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
