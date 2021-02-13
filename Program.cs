namespace HikDownloader
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

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
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            try
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            }
            catch (Exception ex)
            {
                Stop(ex.Message);
            }

            if (!HCNetSDK.Initialize())
            {
                Stop(HCNetSDK.GetLastError());
            }
            else
            {
                if (!HCNetSDK.SetLogToFile(Config.HCNetSDK.Log))
                {
                    Util.ShowWarning(HCNetSDK.GetLastError());
                }

                if (!HCNetSDK.Login(Config.HCNetSDK.Device))
                {
                    Stop(HCNetSDK.GetLastError());
                }
            }
        }

        /// <summary>
        /// Muestra un mensaje y termina el proceso.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public static void Stop(string message)
        {
            Util.ShowError(message);

            Environment.Exit(-1);
        }
    }
}
