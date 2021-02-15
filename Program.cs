namespace HikDownloader
{
    using CommandLine;
    using CommandLine.Text;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Programa.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Configuración.
        /// </summary>
        public static Config.Config Config { get; private set; }

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// <param name="args">Argumentos.</param>
        public static void Main(string[] args)
        {
            SentenceBuilder.Factory = () => new LocalizableSentenceBuilder();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        /// <summary>
        /// Ejecuta la descarga de grabaciones de acuerdo a las opciones especificadas.
        /// </summary>
        /// <param name="opts">Opciones.</param>
        private static void Run(Options opts)
        {
            PrintAppHeader();

            if (InitializeDependencies())
            {
                DownloadRecordings(opts);
            }
        }

        /// <summary>
        /// Imprime la cabecera de la aplicación.
        /// </summary>
        private static void PrintAppHeader()
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
            Console.WriteLine($"{versionInfo.ProductName} v{versionInfo.ProductVersion}\n");
        }

        /// <summary>
        /// Inicializa las dependencias de la aplicación.
        /// </summary>
        private static bool InitializeDependencies()
        {
            try
            {
                Config = JsonConvert.DeserializeObject<Config.Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la configuración: {ex.Message}");

                return false;
            }

            if (HCNetSDK.SDK.Initialize())
            {
                if (HCNetSDK.Log.SetLogToFile(Config.HCNetSDK.Log))
                {
                    if (HCNetSDK.Session.Login(Config.HCNetSDK.Session))
                    {
                        return true;
                    }
                }
            }

            Console.WriteLine($"HCNetSDK: {HCNetSDK.Error.GetLastError()}");

            return false;
        }

        /// <summary>
        /// Descarga grabaciones.
        /// </summary>
        /// <param name="opts">Opciones.</param>
        private static void DownloadRecordings(Options opts)
        {
            var channels = opts.Canal.Distinct().ToArray();
            Array.Sort(channels);

            var recordings = Search.Execute(channels, opts.Desde, opts.Hasta);
            if (recordings.Count > 0)
            {
                if (!opts.Buscar)
                {
                    Download.Execute(recordings);

                    if (opts.Combinar)
                    {
                        Combine.Execute();
                    }
                }
            }

            HCNetSDK.Session.Logout();

            HCNetSDK.SDK.Cleanup();

            Console.WriteLine("Se han completado todas las tareas.");
            Console.WriteLine();
        }
    }
}
