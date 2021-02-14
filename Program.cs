namespace HikDownloader
{
    using CommandLine;
    using CommandLine.Text;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Pastel;

    /// <summary>
    /// Programa.
    /// </summary>
    public static class Program
    {
        #region Cierre de la aplicación
        /// <summary>
        /// Indica que se han llevado a cabo las tareas de limpieza.
        /// </summary>
        private static bool cleanedUp = false;

        /// <summary>
        /// Agrega o quita una función HandlerRoutine definida por la aplicación de la lista de funciones de controlador para el proceso de llamada.
        /// </summary>
        /// <param name="Handler">Puntero a la función HandlerRoutine definida por la aplicación que se va a agregar o quitar. Este parámetro puede ser NULL.</param>
        /// <param name="Add">Si este parámetro es TRUE, se agrega el controlador; si es FALSE, se quita el controlador.</param>
        /// <returns>
        /// Si la función se realiza correctamente, el valor devuelto es distinto de cero.
        /// Si la función no se realiza correctamente, el valor devuelto es cero. Para obtener información de error extendida, llame a GetLastError.
        /// </returns>
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        /// <summary>
        /// Un tipo de delegado que se utilizará como rutina del controlador.
        /// </summary>
        /// <param name="CtrlType">Mensaje de control.</param>
        /// <returns>Si la función maneja la señal de control, debería devolver TRUE. Si devuelve FALSE, se utiliza la siguiente función de controlador en la lista de controladores para este proceso.</returns>
        private delegate bool HandlerRoutine(CtrlTypes CtrlType);

        /// <summary>
        /// Mensajes de control enviados a la rutina del controlador.
        /// </summary>
        private enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        /// <summary>
        /// Rutina del controlador.
        /// </summary>
        /// <param name="CtrlType">Mensaje de control.</param>
        /// <returns>Si la función maneja la señal de control, debería devolver TRUE. Si devuelve FALSE, se utiliza la siguiente función de controlador en la lista de controladores para este proceso.</returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            if (ctrlType == CtrlTypes.CTRL_C_EVENT || ctrlType == CtrlTypes.CTRL_BREAK_EVENT)
            {
                Console.Write("¿Confirma la cancelación de la tarea y cierre de la aplicación? (s/n) ");
                var key = Console.ReadKey();
                if (key.KeyChar.ToString().Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    Cleanup();

                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine();
                }

                return true;
            }
            else if (ctrlType == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                Cleanup();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Responde a la salida del proceso.
        /// </summary>
        /// <param name="sender">Origen del evento.</param>
        /// <param name="e">Datos del evento.</param>
        private static void OnProcessExit(object sender, EventArgs e)
        {
            Cleanup();
        }

        /// <summary>
        /// Ejecuta las tareas de limpieza previas al cierre de la aplicación.
        /// </summary>
        private static void Cleanup()
        {
            if (!cleanedUp)
            {
                cleanedUp = true;

                HCNetSDK.Logout();

                HCNetSDK.Cleanup();
            }
        }
        #endregion

        /// <summary>
        /// Configuración.
        /// </summary>
        public static Config Config { get; private set; }

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
            var location = Assembly.GetExecutingAssembly().Location;
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(location);
            Console.WriteLine($"{versionInfo.ProductName} v{versionInfo.ProductVersion}\n");

            try
            {
                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar la consola: {ex.Message}".Pastel("FFFFFF").PastelBg("FF0000"));

                return;
            }

            try
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la configuración: {ex.Message}".Pastel("FFFFFF").PastelBg("FF0000"));

                return;
            }

            if (HCNetSDK.Initialize())
            {
                if (HCNetSDK.SetLogToFile(Config.HCNetSDK.Log))
                {
                    if (HCNetSDK.Login(Config.HCNetSDK.Device))
                    {
                        var channels = opts.Canal.Distinct().ToArray();
                        Array.Sort(channels);

                        Search(channels, opts.Desde, opts.Hasta);

                        return;
                    }
                }
            }

            Console.WriteLine($"HCNetSDK: {HCNetSDK.GetLastError()}".Pastel("FFFFFF").PastelBg("FF0000"));
        }

        #region Búsqueda
        /// <summary>
        /// Realiza una búsqueda.
        /// </summary>
        /// <param name="channels">Uno o más canales.</param>
        /// <param name="start">Fecha de comienzo.</param>
        /// <param name="end">Fecha de finalización.</param>
        /// <returns>Resultados de la búsqueda.</returns>
        private static List<HCNetSDK.NET_DVR_FINDDATA> Search(int[] channels, DateTime start, DateTime end)
        {
            Console.WriteLine("Configuración de búsqueda:");
            Console.WriteLine($"  > Canales:   {string.Join(", ", channels)}");
            Console.WriteLine($"  > Intervalo: {start:dd/MM/yyyy} a {end:dd/MM/yyyy}\n");

            var total = 0;
            var results = new List<HCNetSDK.NET_DVR_FINDDATA>();
            foreach (var channel in channels)
            {
                var count = 0;
                foreach (var from in EachDay(start, end))
                {
                    var thru = from.AddHours(23).AddMinutes(59).AddSeconds(59);
                    if (thru > end)
                    {
                        thru = end;
                    }

                    var conditions = default(HCNetSDK.NET_DVR_FILECOND);
                    conditions.lChannel = channel;
                    conditions.dwFileType = 0xff;
                    conditions.dwIsLocked = 0xff;

                    conditions.struStartTime.dwYear = (uint)from.Year;
                    conditions.struStartTime.dwMonth = (uint)from.Month;
                    conditions.struStartTime.dwDay = (uint)from.Day;
                    conditions.struStartTime.dwHour = 0;
                    conditions.struStartTime.dwMinute = 0;
                    conditions.struStartTime.dwSecond = 0;

                    conditions.struStopTime.dwYear = (uint)thru.Year;
                    conditions.struStopTime.dwMonth = (uint)thru.Month;
                    conditions.struStopTime.dwDay = (uint)thru.Day;
                    conditions.struStopTime.dwHour = 23;
                    conditions.struStopTime.dwMinute = 59;
                    conditions.struStopTime.dwSecond = 59;

                    var handle = HCNetSDK.FindFile(HCNetSDK.UserId, ref conditions);
                    if (handle > -1)
                    {
                        var record = default(HCNetSDK.NET_DVR_FINDDATA);
                        while (true)
                        {
                            var result = HCNetSDK.FindNextFile(handle, ref record);
                            if (result == HCNetSDK.NET_DVR_ISFINDING)
                            {
                                continue;
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_SUCCESS)
                            {
                                if (!results.Contains(record))
                                {
                                    results.Add(record);

                                    count++;
                                }
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_NOFIND || result == HCNetSDK.NET_DVR_NOMOREFILE)
                            {
                                break;
                            }
                            else if (result == HCNetSDK.NET_DVR_FIND_TIMEOUT)
                            {
                                Console.WriteLine($"  > Canal N° {channel:00}: Tiempo de espera agotado.".Pastel("FFA500"));

                                break;
                            }
                            else if (result == HCNetSDK.NET_DVR_FILE_EXCEPTION)
                            {
                                Console.WriteLine($"  > Canal N° {channel:00}: Error en la búsqueda.".Pastel("FF0000"));

                                break;
                            }
                        }

                        HCNetSDK.FindClose(handle);
                    }
                    else
                    {
                        Console.WriteLine($"  > Canal N° {channel:00}: {HCNetSDK.GetLastError()}".Pastel("FF0000"));
                    }
                }

                if (count > 0)
                {
                    Console.WriteLine($"  > Canal N° {channel:00}: {count:N0} grabaciones.");
                }
                else
                {
                    Console.WriteLine($"  > Canal N° {channel:00}: No se han encontrado grabaciones.");
                }

                total += count;
            }

            if (total > 0)
            {
                Console.WriteLine("\n" + $"  Se han encontrado {total:N0} grabaciones.".Pastel("00FFFF") + "\n");
            }
            else
            {
                Console.WriteLine("\n" + $"  No se han encontrado grabaciones.".Pastel("00FFFF") + "\n");
            }

            return results;
        }

        /// <summary>
        /// Obtiene cada día entre dos fechas.
        /// </summary>
        /// <param name="from">Desde.</param>
        /// <param name="thru">Hasta</param>
        /// <returns>Cada día entre <paramref name="from"/> y <paramref name="thru"/> inclusive.</returns>
        private static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }
        #endregion
    }
}
