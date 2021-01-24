namespace HikDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using FluentDateTime;
    using HCNetSDK;

    /// <summary>
    /// Ventana principal.
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Responde a la carga del formulario.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Downloads == string.Empty)
            {
                Properties.Settings.Default.Downloads = Util.GetDirectory("downloads");
                Properties.Settings.Default.Save();
            }

            for (var i = 1; i <= 16; i++)
            {
                Channels.Items.Add(new ListViewItem
                {
                    Text = Properties.Resources.ResourceManager.GetString("Channel" + i),
                    Tag = new Channel(i),
                });
            }

            columnHeader6.Width = Events.Width - 25;

            Period.SelectedIndex = 3; // Semana anterior

            DownloadDir.Text = Properties.Settings.Default.Downloads;
        }

        /// <summary>
        /// Inicializa el entorno de HCNetSDK.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private async void MainWindow_Shown(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                if (SDK.Initialize())
                {
                    LogMessage("Se ha inicializado el entorno de programación.");

                    if (SDK.EnableLogging(Util.GetDirectory("logs")))
                    {
                        LogMessage("Se ha habilitado el registro de mensajes de la SDK.");

                        Session.Address = Properties.Settings.Default.Address;
                        Session.Port = Properties.Settings.Default.Port;

                        if (Session.Login(Properties.Settings.Default.UserName, Properties.Settings.Default.Password))
                        {
                            LogMessage(string.Format("Se ha iniciado sesión (ID: {0}).", Session.User.Identifier));

                            Invoke(new MethodInvoker(delegate
                            {
                                Enabled = true;
                            }));

                            return;
                        }
                        else
                        {
                            LogMessage(string.Format("Error al iniciar sesión (Código: {0}).", SDK.GetLastError()));
                        }
                    }
                    else
                    {
                        LogMessage(string.Format("Error al habilitar el registro de mensajes (Código: {0}).", SDK.GetLastError()));
                    }
                }
                else
                {
                    LogMessage(string.Format("Error al inicializar el entorno de programación (Código: {0}).", SDK.GetLastError()));
                }
            });
        }

        /// <summary>
        /// Evita que se modifique el ancho de las columnas.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Recordings_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = Recordings.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Guarda el valor validado en las propiedades del control.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Start_Validated(object sender, EventArgs e)
        {
            Start.Tag = Start.Value;
        }

        /// <summary>
        /// Valida que la fecha de comienzo es menor o igual que la fecha de finalización.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Start_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Start.Value >= End.Value)
            {
                MessageBox.Show("La fecha de comienzo debe ser menor a la fecha de finalización.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                e.Cancel = true;
                Start.Value = (DateTime)Start.Tag;
            }
        }

        /// <summary>
        /// Guarda el valor validado en las propiedades del control.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void End_Validated(object sender, EventArgs e)
        {
            End.Tag = End.Value;
        }

        /// <summary>
        /// Valida que la fecha de finalización es mayor o igual que la fecha de comienzo.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void End_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (End.Value <= Start.Value)
            {
                MessageBox.Show("La fecha de finalización debe ser mayor a la fecha de comienzo.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                e.Cancel = true;
                End.Value = (DateTime)End.Tag;
            }
        }

        /// <summary>
        /// Responde al cambio de período.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Period_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Period.SelectedIndex)
            {
                case 0:
                    Start.Value = DateTime.Today.Midnight();
                    End.Value = DateTime.Today.EndOfDay();
                    break;

                case 1:
                    Start.Value = DateTime.Today.PreviousDay().Midnight();
                    End.Value = DateTime.Today.PreviousDay().EndOfDay();
                    break;

                case 2:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1);
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case 3:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1).WeekEarlier();
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case 4:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(-13);
                    End.Value = DateTime.Today.WeekEarlier().LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case 5:
                    Start.Value = DateTime.Today.BeginningOfMonth();
                    End.Value = DateTime.Today.EndOfMonth();
                    break;

                case 6:
                    Start.Value = DateTime.Today.PreviousMonth().BeginningOfMonth();
                    End.Value = DateTime.Today.PreviousMonth().EndOfMonth();
                    break;
            }

            Start.Enabled = Period.SelectedIndex == 7;
            End.Enabled = Period.SelectedIndex == 7;

            Start.Tag = Start.Value;
            End.Tag = End.Value;
        }

        /// <summary>
        /// Cambia el directorio de descarga.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                if (DownloadDir.Text != string.Empty)
                {
                    dialog.SelectedPath = DownloadDir.Text;
                }

                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    DownloadDir.Text = dialog.SelectedPath;

                    Properties.Settings.Default.Downloads = dialog.SelectedPath;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Cambia el estado del botón de búsqueda.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Channels_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Search.Enabled = Channels.CheckedIndices.Count > 0;
        }

        /// <summary>
        /// Ejecuta o cancela una búsqueda de grabaciones.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private async void Search_Click(object sender, EventArgs e)
        {
            if (Searcher.IsRunning)
            {
                Searcher.Cancel();

                return;
            }

            var channels = new List<Channel>();
            foreach (var item in Channels.CheckedItems)
            {
                channels.Add((Channel)((ListViewItem)item).Tag);
            }

            await Task.Run(() =>
            {
                Searcher.Search(channels.ToArray(), Start.Value, End.Value);
            });
        }

        /// <summary>
        /// Agrega un mensaje al registro de eventos.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        private void LogMessage(string message)
        {
            if (Events.InvokeRequired)
            {
                Events.Invoke(new MethodInvoker(delegate
                {
                    Events.Items.Add(message);
                }));
            }
            else
            {
                Events.Items.Add(message);
            }
        }
    }
}
