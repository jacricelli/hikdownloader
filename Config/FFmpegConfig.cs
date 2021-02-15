using System.IO;

namespace HikDownloader.Config
{
    /// <summary>
    /// Configuración de FFmpeg.
    /// </summary>
    public class FFmpegConfig
    {
        /// <summary>
        /// Ruta al ejecutable de FFmpeg.
        /// </summary>
        private string _bin;

        /// <summary>
        /// Ruta donde se guardan los archivos combinados.
        /// </summary>
        private string _dir;

        /// <summary>
        /// Cantidad de tareas a ejecutar en paralelo.
        /// </summary>
        private int _simultaneousTasks = 3;

        /// <summary>
        /// Ruta al ejecutable de FFmpeg.
        /// </summary>
        public string Bin
        {
            get
            {
                return _bin;
            }

            set
            {
                _bin = value.Replace("{appPath}", Util.GetPath());
            }
        }

        /// <summary>
        /// Obtiene o establece la ruta donde se guardan los archivos combinados.
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
        /// Obtiene o establece la cantidad de tareas a ejecutar en paralelo.
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
