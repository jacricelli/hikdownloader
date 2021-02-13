namespace HikDownloader
{
    using Pastel;
    using System;
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

        /// <summary>
        /// Muestra un mensaje de éxito.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public static void ShowSuccess(string message)
        {
            Console.WriteLine($"{message.TrimEnd('.')}.".Pastel("bab86c"));
        }

        /// <summary>
        /// Muestra un mensaje de información.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public static void ShowInfo(string message)
        {
            Console.WriteLine($"{message.TrimEnd('.')}.".Pastel("00FFFF"));
        }

        /// <summary>
        /// Mensaje un mensaje de advertencia.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public static void ShowWarning(string message)
        {
            Console.WriteLine($"{message.TrimEnd('.')}.".Pastel("FFFFFF").PastelBg("ffa500"));
        }

        /// <summary>
        /// Muestra un mensaje de error.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public static void ShowError(string message)
        {
            Console.WriteLine($"{message.TrimEnd('.')}.".Pastel("FFFFFF").PastelBg("FF0000"));
        }
    }
}
