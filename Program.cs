namespace HikDownloader
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    public static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\93e38211-85fd-4c62-9fb9-6e17b9d82580"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Ya hay una instancia de la aplicación en ejecución.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
            }
        }
    }
}
