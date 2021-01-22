namespace HikDownloader
{
    using System;
    using System.Windows.Forms;
    using FluentDateTime;

    /// <summary>
    /// Ventana principal.
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// Canales.
        /// </summary>
        private CheckBox[] channels;

        /// <summary>
        /// Tipos de búsqueda.
        /// </summary>
        private enum searchType : int
        {
            today = 0,
            yesterday = 1,
            thisWeek = 2,
            lastWeek = 3,
            last2Weeks = 4,
            thisMonth = 5,
            lastMonth = 6,
            customDate = 7,
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            channels = new CheckBox[]
            {
                Channel1, Channel2, Channel3, Channel4, Channel5, Channel6, Channel6, Channel7, Channel8,
                Channel9, Channel10, Channel11, Channel12, Channel13, Channel14, Channel15, Channel16,
            };

            if (Properties.Settings.Default.Downloads == string.Empty)
            {
                Properties.Settings.Default.Downloads = Util.GetDirectory("downloads");
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Responde a la carga del formulario.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Period.SelectedIndex = (int)searchType.lastWeek;

            DownloadDir.Text = Properties.Settings.Default.Downloads;
        }

        /// <summary>
        /// Inicializa el entorno de HCNetSDK.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (HCNetSDK.Initialize())
            {
                if (HCNetSDK.EnableLogging(Util.GetDirectory("logs")))
                {
                    return;
                }
                else
                {
                    MessageBox.Show(string.Format("Error #{0} al habilitar el registro de mensajes.", HCNetSDK.GetLastError()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(string.Format("Error #{0} al inicializar el entorno de programación.", HCNetSDK.GetLastError()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        /// Habilita el botón de búsqueda si al menos una casilla de verificación ha sido marcada.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Channels_CheckChanged(object sender, EventArgs e)
        {
            var state = false;
            foreach (var channel in channels)
            {
                if (channel.Checked)
                {
                    state = true;
                    break;
                }
            }

            Search.Enabled = state;
        }

        /// <summary>
        /// Responde al cambio de período.
        /// </summary>
        /// <param name="sender">Origen del evento</param>
        /// <param name="e">Datos del evento.</param>
        private void Period_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIndex = (searchType)Period.SelectedIndex;
            switch (selectedIndex)
            {
                case searchType.today:
                    Start.Value = DateTime.Today.Midnight();
                    End.Value = DateTime.Today.EndOfDay();
                    break;

                case searchType.yesterday:
                    Start.Value = DateTime.Today.PreviousDay().Midnight();
                    End.Value = DateTime.Today.PreviousDay().EndOfDay();
                    break;

                case searchType.thisWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1);
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case searchType.lastWeek:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(1).WeekEarlier();
                    End.Value = Start.Value.LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case searchType.last2Weeks:
                    Start.Value = DateTime.Today.FirstDayOfWeek().AddDays(-13);
                    End.Value = DateTime.Today.WeekEarlier().LastDayOfWeek().AddDays(1).EndOfDay();
                    break;

                case searchType.thisMonth:
                    Start.Value = DateTime.Today.BeginningOfMonth();
                    End.Value = DateTime.Today.EndOfMonth();
                    break;

                case searchType.lastMonth:
                    Start.Value = DateTime.Today.PreviousMonth().BeginningOfMonth();
                    End.Value = DateTime.Today.PreviousMonth().EndOfMonth();
                    break;
            }

            Start.Enabled = selectedIndex == searchType.customDate;
            End.Enabled = selectedIndex == searchType.customDate;

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
                }
            }
        }
    }
}
