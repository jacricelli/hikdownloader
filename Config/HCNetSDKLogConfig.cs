﻿using System.IO;

namespace HikDownloader.Config
{
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
}
