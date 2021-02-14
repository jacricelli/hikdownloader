namespace HikDownloader
{
    using System.IO;

    /// <summary>
    /// Configuración.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Configuración de HCNetSDK.
        /// </summary>
        public HCNetSDKConfig HCNetSDK { get; set; }

        /// <summary>
        /// Configuración de HikDownloader.
        /// </summary>
        public HikDownloaderConfig HikDownloader { get; set; }
    }

    /// <summary>
    /// Configuración de HCNetSDK.
    /// </summary>
    public class HCNetSDKConfig
    {
        /// <summary>
        /// Configuración del registro de mensajes.
        /// </summary>
        public HCNetSDKLogConfig Log { get; set; }

        /// <summary>
        /// Configuración del dispositivo.
        /// </summary>
        public HCNetSDKDeviceConfig Device { get; set; }
    }

    /// <summary>
    /// Configuración del registro de mensajes.
    /// </summary>
    public class HCNetSDKLogConfig
    {
        /// <summary>
        /// Ruta de acceso donde se guardan los archivos de registro.
        /// </summary>
        private string _dir;

        /// <summary>
        /// Obtiene o establece la ruta de acceso donde se guardan los archivos de registro.
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
        /// Obtiene o establece el nivel de registro de mensajes.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Obtiene o establece la eliminación automática de archivos de registro cuando se alcance el número máximo.
        /// </summary>
        public bool AutoDel { get; set; }
    }

    /// <summary>
    /// Configuración del dispositivo.
    /// </summary>
    public class HCNetSDKDeviceConfig
    {
        /// <summary>
        /// Dirección.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Puerto.
        /// </summary>
        public int Port { get; set; } = 8000;

        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Contraseña.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Configuración de HikDownloader.
    /// </summary>
    public class HikDownloaderConfig
    {
        /// <summary>
        /// Configuración de las descargas.
        /// </summary>
        public HikDownloaderDownloadConfig Downloads { get; set; }
    }

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
        private int _parallel = 1;
        
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
        public int Parallel
        {
            get
            {
                return _parallel;
            }

            set
            {
                _parallel = value < 1 ? 1 : value;
            }
        }
    }
}