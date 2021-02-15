namespace HikDownloader.Config
{
    using System.IO;

    /// <summary>
    /// Configuración de las descargas.
    /// </summary>
    public class HikDownloaderDownloadConfig
    {
        /// <summary>
        /// Ruta de acceso donde se guardan las descargas.
        /// </summary>
        private string _dir;

        /// <summary>
        /// Cantidad de descargas a ejecutar en paralelo.
        /// </summary>
        private int _simultaneousTasks = 3;

        /// <summary>
        /// Obtiene o establece la ruta de acceso donde se guardan las descargas.
        /// </summary>
        public string Dir
        {
            get
            {
                return _dir;
            }

            set
            {
                _dir = value.Replace("{appPath}", Util.GetPath());
                Directory.CreateDirectory(_dir);
            }
        }

        /// <summary>
        /// Obtiene o establece la cantidad de descargas a ejecutar en paralelo.
        /// </summary>
        public int SimultaneousTasks
        {
            get
            {
                return _simultaneousTasks;
            }

            set
            {
                _simultaneousTasks = value < 1 ? 1 : value;
            }
        }
    }
}
