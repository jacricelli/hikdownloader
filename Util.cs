namespace HikDownloader
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Colección de métodos utilizados por la aplicación.
    /// </summary>
    public static class Util
    {
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
        /// Formatea tamaños de datos en formas legibles por humanos.
        /// </summary>
        /// <param name="bytes">Tamaño en bytes.</param>
        /// <returns>Tamaño legible por humanos.</returns>
        public static string ToReadableSize(ulong bytes)
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
