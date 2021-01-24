namespace HikDownloader.HCNetSDK
{
    using System;

    /// <summary>
    /// Video.
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Obtiene el nombre del archivo.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Obtiene el tamaño del archivo.
        /// </summary>
        public uint FileSize { get; private set; }

        /// <summary>
        /// Obtiene el tamaño del archivo con el sufijo correspondiente.
        /// </summary>
        public string FileSizeWithPrefix => ToReadableSize(FileSize);

        /// <summary>
        /// Obtiene el comienzo.
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// Obtiene la finalización.
        /// </summary>
        public DateTime End { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <param name="fileSize">Tamaño del archivo.</param>
        /// <param name="start">Comienzo.</param>
        /// <param name="end">Finalización.</param>
        public Video(string fileName, uint fileSize, DateTime start, DateTime end)
        {
            FileName = fileName;
            FileSize = fileSize;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Formatea tamaños de datos en formas legibles por humanos.
        /// </summary>
        /// <param name="bytes">Tamaño en bytes.</param>
        /// <returns>Tamaño legible por humanos.</returns>
        private string ToReadableSize(uint bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
