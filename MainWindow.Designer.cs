namespace HikDownloader
{
    partial class MainWindow
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            HCNetSDK.Cleanup();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Download = new System.Windows.Forms.Button();
            this.Browse = new System.Windows.Forms.Button();
            this.DownloadDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Recordings = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Search = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Period = new System.Windows.Forms.ComboBox();
            this.End = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.Start = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Channel16 = new System.Windows.Forms.CheckBox();
            this.Channel15 = new System.Windows.Forms.CheckBox();
            this.Channel13 = new System.Windows.Forms.CheckBox();
            this.Channel14 = new System.Windows.Forms.CheckBox();
            this.Channel11 = new System.Windows.Forms.CheckBox();
            this.Channel12 = new System.Windows.Forms.CheckBox();
            this.Channel10 = new System.Windows.Forms.CheckBox();
            this.Channel9 = new System.Windows.Forms.CheckBox();
            this.Channel8 = new System.Windows.Forms.CheckBox();
            this.Channel7 = new System.Windows.Forms.CheckBox();
            this.Channel6 = new System.Windows.Forms.CheckBox();
            this.Channel5 = new System.Windows.Forms.CheckBox();
            this.Channel4 = new System.Windows.Forms.CheckBox();
            this.Channel3 = new System.Windows.Forms.CheckBox();
            this.Channel2 = new System.Windows.Forms.CheckBox();
            this.Channel1 = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Download);
            this.groupBox3.Controls.Add(this.Browse);
            this.groupBox3.Controls.Add(this.DownloadDir);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.Recordings);
            this.groupBox3.Location = new System.Drawing.Point(341, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(704, 387);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Grabaciones";
            // 
            // Download
            // 
            this.Download.Enabled = false;
            this.Download.Location = new System.Drawing.Point(574, 348);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(123, 32);
            this.Download.TabIndex = 8;
            this.Download.Text = "&Descargar";
            this.Download.UseVisualStyleBackColor = true;
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(389, 356);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(40, 24);
            this.Browse.TabIndex = 3;
            this.Browse.Text = "...";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // DownloadDir
            // 
            this.DownloadDir.Location = new System.Drawing.Point(8, 356);
            this.DownloadDir.Name = "DownloadDir";
            this.DownloadDir.ReadOnly = true;
            this.DownloadDir.Size = new System.Drawing.Size(376, 23);
            this.DownloadDir.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 336);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Guardar grabaciones en:";
            // 
            // Recordings
            // 
            this.Recordings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.Recordings.FullRowSelect = true;
            this.Recordings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.Recordings.HideSelection = false;
            this.Recordings.Location = new System.Drawing.Point(8, 24);
            this.Recordings.MultiSelect = false;
            this.Recordings.Name = "Recordings";
            this.Recordings.Size = new System.Drawing.Size(688, 304);
            this.Recordings.TabIndex = 0;
            this.Recordings.UseCompatibleStateImageBehavior = false;
            this.Recordings.View = System.Windows.Forms.View.Details;
            this.Recordings.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.Recordings_ColumnWidthChanging);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Archivo";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Tamaño";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Comienzo";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Finalización";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Estado";
            this.columnHeader5.Width = 100;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Search);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.Period);
            this.groupBox2.Controls.Add(this.End);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.Start);
            this.groupBox2.Location = new System.Drawing.Point(8, 240);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(320, 155);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Búsqueda";
            // 
            // Search
            // 
            this.Search.Enabled = false;
            this.Search.Location = new System.Drawing.Point(96, 110);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(128, 32);
            this.Search.TabIndex = 4;
            this.Search.Text = "&Buscar grabaciones";
            this.Search.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Finalización";
            // 
            // Period
            // 
            this.Period.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Period.FormattingEnabled = true;
            this.Period.Items.AddRange(new object[] {
            "Hoy",
            "Ayer",
            "Esta semana",
            "Semana anterior",
            "Últimas 2 semanas",
            "Este mes",
            "Mes anterior",
            "Rango personalizado"});
            this.Period.Location = new System.Drawing.Point(8, 22);
            this.Period.Name = "Period";
            this.Period.Size = new System.Drawing.Size(304, 23);
            this.Period.TabIndex = 4;
            this.Period.SelectedIndexChanged += new System.EventHandler(this.Period_SelectedIndexChanged);
            // 
            // End
            // 
            this.End.Checked = false;
            this.End.CustomFormat = " dd-MM-yyyy HH:mm:ss";
            this.End.Enabled = false;
            this.End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.End.Location = new System.Drawing.Point(166, 75);
            this.End.Name = "End";
            this.End.Size = new System.Drawing.Size(146, 23);
            this.End.TabIndex = 6;
            this.End.Validating += new System.ComponentModel.CancelEventHandler(this.End_Validating);
            this.End.Validated += new System.EventHandler(this.End_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Comienzo";
            // 
            // Start
            // 
            this.Start.Checked = false;
            this.Start.CustomFormat = " dd-MM-yyyy HH:mm:ss";
            this.Start.Enabled = false;
            this.Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.Start.Location = new System.Drawing.Point(8, 75);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(146, 23);
            this.Start.TabIndex = 4;
            this.Start.Validating += new System.ComponentModel.CancelEventHandler(this.Start_Validating);
            this.Start.Validated += new System.EventHandler(this.Start_Validated);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Channel16);
            this.groupBox1.Controls.Add(this.Channel15);
            this.groupBox1.Controls.Add(this.Channel13);
            this.groupBox1.Controls.Add(this.Channel14);
            this.groupBox1.Controls.Add(this.Channel11);
            this.groupBox1.Controls.Add(this.Channel12);
            this.groupBox1.Controls.Add(this.Channel10);
            this.groupBox1.Controls.Add(this.Channel9);
            this.groupBox1.Controls.Add(this.Channel8);
            this.groupBox1.Controls.Add(this.Channel7);
            this.groupBox1.Controls.Add(this.Channel6);
            this.groupBox1.Controls.Add(this.Channel5);
            this.groupBox1.Controls.Add(this.Channel4);
            this.groupBox1.Controls.Add(this.Channel3);
            this.groupBox1.Controls.Add(this.Channel2);
            this.groupBox1.Controls.Add(this.Channel1);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 228);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cámaras";
            // 
            // Channel16
            // 
            this.Channel16.AutoSize = true;
            this.Channel16.Location = new System.Drawing.Point(176, 197);
            this.Channel16.Name = "Channel16";
            this.Channel16.Size = new System.Drawing.Size(105, 19);
            this.Channel16.TabIndex = 18;
            this.Channel16.Tag = "16";
            this.Channel16.Text = "16 - Cámara 16";
            this.Channel16.UseVisualStyleBackColor = true;
            this.Channel16.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel15
            // 
            this.Channel15.AutoSize = true;
            this.Channel15.Location = new System.Drawing.Point(176, 172);
            this.Channel15.Name = "Channel15";
            this.Channel15.Size = new System.Drawing.Size(143, 19);
            this.Channel15.TabIndex = 17;
            this.Channel15.Tag = "15";
            this.Channel15.Text = "15 - Sector lubricantes";
            this.Channel15.UseVisualStyleBackColor = true;
            this.Channel15.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel13
            // 
            this.Channel13.AutoSize = true;
            this.Channel13.Location = new System.Drawing.Point(176, 122);
            this.Channel13.Name = "Channel13";
            this.Channel13.Size = new System.Drawing.Size(132, 19);
            this.Channel13.TabIndex = 16;
            this.Channel13.Tag = "13";
            this.Channel13.Text = "13 - Calle Fumadero";
            this.Channel13.UseVisualStyleBackColor = true;
            this.Channel13.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel14
            // 
            this.Channel14.AutoSize = true;
            this.Channel14.Location = new System.Drawing.Point(176, 147);
            this.Channel14.Name = "Channel14";
            this.Channel14.Size = new System.Drawing.Size(131, 19);
            this.Channel14.TabIndex = 15;
            this.Channel14.Tag = "14";
            this.Channel14.Text = "14 - Acceso portería";
            this.Channel14.UseVisualStyleBackColor = true;
            this.Channel14.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel11
            // 
            this.Channel11.AutoSize = true;
            this.Channel11.Location = new System.Drawing.Point(176, 72);
            this.Channel11.Name = "Channel11";
            this.Channel11.Size = new System.Drawing.Size(82, 19);
            this.Channel11.TabIndex = 14;
            this.Channel11.Tag = "11";
            this.Channel11.Text = "11 - Kase 1";
            this.Channel11.UseVisualStyleBackColor = true;
            this.Channel11.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel12
            // 
            this.Channel12.AutoSize = true;
            this.Channel12.Location = new System.Drawing.Point(176, 97);
            this.Channel12.Name = "Channel12";
            this.Channel12.Size = new System.Drawing.Size(120, 19);
            this.Channel12.TabIndex = 13;
            this.Channel12.Tag = "12";
            this.Channel12.Text = "12 - SFM 4 - BTA 1";
            this.Channel12.UseVisualStyleBackColor = true;
            this.Channel12.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel10
            // 
            this.Channel10.AutoSize = true;
            this.Channel10.Location = new System.Drawing.Point(176, 47);
            this.Channel10.Name = "Channel10";
            this.Channel10.Size = new System.Drawing.Size(103, 19);
            this.Channel10.TabIndex = 12;
            this.Channel10.Tag = "10";
            this.Channel10.Text = "10 - CCM 4 y 8";
            this.Channel10.UseVisualStyleBackColor = true;
            this.Channel10.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel9
            // 
            this.Channel9.AutoSize = true;
            this.Channel9.Location = new System.Drawing.Point(176, 22);
            this.Channel9.Name = "Channel9";
            this.Channel9.Size = new System.Drawing.Size(115, 19);
            this.Channel9.TabIndex = 11;
            this.Channel9.Tag = "9";
            this.Channel9.Text = "09 - CCM 7, 8 y 9";
            this.Channel9.UseVisualStyleBackColor = true;
            this.Channel9.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel8
            // 
            this.Channel8.AutoSize = true;
            this.Channel8.Location = new System.Drawing.Point(13, 197);
            this.Channel8.Name = "Channel8";
            this.Channel8.Size = new System.Drawing.Size(133, 19);
            this.Channel8.TabIndex = 10;
            this.Channel8.Tag = "8";
            this.Channel8.Text = "08 - CCM 1 y CCM 3";
            this.Channel8.UseVisualStyleBackColor = true;
            this.Channel8.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel7
            // 
            this.Channel7.AutoSize = true;
            this.Channel7.Location = new System.Drawing.Point(13, 172);
            this.Channel7.Name = "Channel7";
            this.Channel7.Size = new System.Drawing.Size(134, 19);
            this.Channel7.TabIndex = 9;
            this.Channel7.Tag = "7";
            this.Channel7.Text = "07 - Lugar de estibas";
            this.Channel7.UseVisualStyleBackColor = true;
            this.Channel7.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel6
            // 
            this.Channel6.AutoSize = true;
            this.Channel6.Location = new System.Drawing.Point(13, 147);
            this.Channel6.Name = "Channel6";
            this.Channel6.Size = new System.Drawing.Size(157, 19);
            this.Channel6.TabIndex = 8;
            this.Channel6.Tag = "6";
            this.Channel6.Text = "06 - Producto en tránsito";
            this.Channel6.UseVisualStyleBackColor = true;
            this.Channel6.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel5
            // 
            this.Channel5.AutoSize = true;
            this.Channel5.Location = new System.Drawing.Point(13, 122);
            this.Channel5.Name = "Channel5";
            this.Channel5.Size = new System.Drawing.Size(125, 19);
            this.Channel5.TabIndex = 7;
            this.Channel5.Tag = "5";
            this.Channel5.Text = "05 - Playa de carga";
            this.Channel5.UseVisualStyleBackColor = true;
            this.Channel5.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel4
            // 
            this.Channel4.AutoSize = true;
            this.Channel4.Location = new System.Drawing.Point(13, 97);
            this.Channel4.Name = "Channel4";
            this.Channel4.Size = new System.Drawing.Size(147, 19);
            this.Channel4.TabIndex = 6;
            this.Channel4.Tag = "4";
            this.Channel4.Text = "04 - Taller Manteniento";
            this.Channel4.UseVisualStyleBackColor = true;
            this.Channel4.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel3
            // 
            this.Channel3.AutoSize = true;
            this.Channel3.Location = new System.Drawing.Point(13, 72);
            this.Channel3.Name = "Channel3";
            this.Channel3.Size = new System.Drawing.Size(125, 19);
            this.Channel3.TabIndex = 5;
            this.Channel3.Tag = "3";
            this.Channel3.Text = "03 - Playa de carga";
            this.Channel3.UseVisualStyleBackColor = true;
            this.Channel3.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel2
            // 
            this.Channel2.AutoSize = true;
            this.Channel2.Location = new System.Drawing.Point(13, 47);
            this.Channel2.Name = "Channel2";
            this.Channel2.Size = new System.Drawing.Size(81, 19);
            this.Channel2.TabIndex = 4;
            this.Channel2.Tag = "2";
            this.Channel2.Text = "02 - Desco";
            this.Channel2.UseVisualStyleBackColor = true;
            this.Channel2.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // Channel1
            // 
            this.Channel1.AutoSize = true;
            this.Channel1.Location = new System.Drawing.Point(13, 22);
            this.Channel1.Name = "Channel1";
            this.Channel1.Size = new System.Drawing.Size(99, 19);
            this.Channel1.TabIndex = 3;
            this.Channel1.Tag = "1";
            this.Channel1.Text = "01 - Comedor";
            this.Channel1.UseVisualStyleBackColor = true;
            this.Channel1.CheckedChanged += new System.EventHandler(this.Channels_CheckChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 406);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HikDownloader";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox DownloadDir;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView Recordings;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Search;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Period;
        private System.Windows.Forms.DateTimePicker End;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker Start;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Channel16;
        private System.Windows.Forms.CheckBox Channel15;
        private System.Windows.Forms.CheckBox Channel13;
        private System.Windows.Forms.CheckBox Channel14;
        private System.Windows.Forms.CheckBox Channel11;
        private System.Windows.Forms.CheckBox Channel12;
        private System.Windows.Forms.CheckBox Channel10;
        private System.Windows.Forms.CheckBox Channel9;
        private System.Windows.Forms.CheckBox Channel8;
        private System.Windows.Forms.CheckBox Channel7;
        private System.Windows.Forms.CheckBox Channel6;
        private System.Windows.Forms.CheckBox Channel5;
        private System.Windows.Forms.CheckBox Channel4;
        private System.Windows.Forms.CheckBox Channel3;
        private System.Windows.Forms.CheckBox Channel2;
        private System.Windows.Forms.CheckBox Channel1;
    }
}

