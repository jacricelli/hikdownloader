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
    }
}
