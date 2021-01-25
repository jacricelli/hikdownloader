namespace HikDownloader
{
    using HikDownloader.HCNetSDK;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Colección de métodos utilizados por la aplicación.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Estado de la descarga.
        /// </summary>
        public enum DownloadStatus : int
        {
            /// <summary>
            /// Completada.
            /// </summary>
            completed = 0,

            /// <summary>
            /// Incompleta.
            /// </summary>
            incomplete = 1,

            /// <summary>
            /// Pendiente.
            /// </summary>
            pending = 2
        }

        /// <summary>
        /// Obtiene el directorio actual desde donde se ejecuta la aplicación.
        /// </summary>
        /// <param name="subdir">Especifica el nombre de un subdirectorio.</param>
        /// <returns>Una cadena con la ruta de acceso al directorio.</returns>
        public static string GetDirectory(string subdir = "")
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (subdir != string.Empty)
            {
                path += "\\" + subdir;
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Obtiene la ruta absoluta para una grabación.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <param name="extension">Extensión del archivo.</param>
        /// <returns>Ruta absoluta.</returns>
        public static string GetRecordingPath(Recording recording, string extension)
        {
            return string.Format(
                Properties.Settings.Default.Downloads,
                "{0}\\{1}\\{2:00}\\Channel{3:00}_{4}_{5}_{6}.{7}",
                recording.Video.Start.Year,
                recording.Video.Start.Month,
                recording.Channel.Number,
                recording.Video.Start.ToString("yyyy-MM-dd"),
                recording.Video.Start.ToString("hhmmss"),
                recording.Video.End.ToString("hhmmss"),
                extension);
        }

        /// <summary>
        /// Comprueba el estado de una descarga.
        /// </summary>
        /// <param name="recording">Grabación.</param>
        /// <returns>Estado de la descarga.</returns>
        public static DownloadStatus CheckDownloadStatus(Recording recording)
        {
            if (File.Exists(GetRecordingPath(recording, "avi")))
            {
                return DownloadStatus.completed;
            }
            else if (File.Exists(GetRecordingPath(recording, "tmp")))
            {
                return DownloadStatus.incomplete;
            }
            else
            {
                return DownloadStatus.pending;
            }
        }
    }
}
