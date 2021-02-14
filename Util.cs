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
        /// Obtiene la ruta de acceso al directorio desde donde se ejecuta la aplicación.
        /// </summary>
        /// <param name="subdir">Subdirectorio a anexar a la ruta.</param>
        /// <param name="file">Archivo a anexar a la ruta.</param>
        /// <returns>Ruta de acceso al directorio, subdirectorio o archivo.</returns>
        public static string GetPath(string subdir = "", string file = "")
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (subdir != string.Empty)
            {
                path += "\\" + subdir;
                Directory.CreateDirectory(path);
            }

            if (file != string.Empty)
            {
                path += "\\" + file;
            }

            return path;
        }
    }
}
