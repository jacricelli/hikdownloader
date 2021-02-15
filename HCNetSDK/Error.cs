namespace HikDownloader.HCNetSDK
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Error.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// La descripción de los errores de HCNetSDK.
        /// </summary>
        private static Dictionary<uint, string> _errors = new Dictionary<uint, string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        static Error()
        {
            var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("HikDownloader.Resources.HCNetSDKErrors.txt");
            using (var reader = new StreamReader(resource))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var error = line.Split('\t');
                    _errors.Add(uint.Parse(error[0]), error[1]);
                }
            }
        }

        /// <summary>
        /// Obtiene el código de error generado por la última operación.
        /// </summary>
        /// <returns>El código de error de la última operación.</returns>
        [DllImport(@"lib\HCNetSDK.dll", EntryPoint = "NET_DVR_GetLastError")]
        public static extern uint GetLastErrno();

        /// <summary>
        /// Obtiene la descripción del error generado por la última operación.
        /// </summary>
        /// <returns>La descripción del error de la última operación.</returns>
        public static string GetLastError()
        {
            var errno = GetLastErrno();
            if (_errors.ContainsKey(errno))
            {
                return _errors[errno];
            }

            return $"El código de error '{errno}' no se encuentra documentado.";
        }
    }
}
