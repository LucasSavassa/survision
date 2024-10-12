namespace UserInterface
{
    partial class SurgeryWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tab = new TabControl();
            tabRecord = new TabPage();
            numRoom = new NumericUpDown();
            imgCapture = new PictureBox();
            btnStop = new Button();
            btnStart = new Button();
            lblRoom = new Label();
            tabRecordings = new TabPage();
            btnExportSurgeryDesc = new Button();
            btnExportJson = new Button();
            numRoom2 = new NumericUpDown();
            lblRoom2 = new Label();
            lisView = new ListView();
            colStart = new ColumnHeader();
            colDuration = new ColumnHeader();
            colShots = new ColumnHeader();
            colPath = new ColumnHeader();
            dtpDay = new DateTimePicker();
            lblDay = new Label();
            tabOptions = new TabPage();
            ckbDemo = new CheckBox();
            lblDemo = new Label();
            btnSave = new Button();
            selNeuralNet = new ComboBox();
            lblNeuralNet = new Label();
            numInterval = new NumericUpDown();
            lblInterval = new Label();
            saveFileDialog1 = new SaveFileDialog();
            tab.SuspendLayout();
            tabRecord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).BeginInit();
            tabRecordings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom2).BeginInit();
            tabOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            SuspendLayout();
            // 
            // tab
            // 
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tab.Controls.Add(tabRecord);
            tab.Controls.Add(tabRecordings);
            tab.Controls.Add(tabOptions);
            tab.Location = new Point(12, 12);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(758, 429);
            tab.TabIndex = 0;
            // 
            // tabRecord
            // 
            tabRecord.Controls.Add(numRoom);
            tabRecord.Controls.Add(imgCapture);
            tabRecord.Controls.Add(btnStop);
            tabRecord.Controls.Add(btnStart);
            tabRecord.Controls.Add(lblRoom);
            tabRecord.Location = new Point(4, 29);
            tabRecord.Name = "tabRecord";
            tabRecord.Padding = new Padding(3);
            tabRecord.Size = new Size(750, 396);
            tabRecord.TabIndex = 0;
            tabRecord.Text = "Gravar";
            tabRecord.UseVisualStyleBackColor = true;
            // 
            // numRoom
            // 
            numRoom.Location = new Point(6, 26);
            numRoom.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRoom.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom.Name = "numRoom";
            numRoom.Size = new Size(125, 27);
            numRoom.TabIndex = 5;
            numRoom.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // imgCapture
            // 
            imgCapture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imgCapture.BackColor = Color.DarkGray;
            imgCapture.Location = new Point(137, 6);
            imgCapture.Name = "imgCapture";
            imgCapture.Size = new Size(607, 384);
            imgCapture.SizeMode = PictureBoxSizeMode.Zoom;
            imgCapture.TabIndex = 4;
            imgCapture.TabStop = false;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Enabled = false;
            btnStop.Location = new Point(6, 361);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(125, 29);
            btnStop.TabIndex = 3;
            btnStop.Text = "Parar";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.Location = new Point(6, 326);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(125, 29);
            btnStart.TabIndex = 2;
            btnStart.Text = "Iniciar";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(3, 3);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(37, 20);
            lblRoom.TabIndex = 0;
            lblRoom.Text = "Sala";
            // 
            // tabRecordings
            // 
            tabRecordings.Controls.Add(btnExportSurgeryDesc);
            tabRecordings.Controls.Add(btnExportJson);
            tabRecordings.Controls.Add(numRoom2);
            tabRecordings.Controls.Add(lblRoom2);
            tabRecordings.Controls.Add(lisView);
            tabRecordings.Controls.Add(dtpDay);
            tabRecordings.Controls.Add(lblDay);
            tabRecordings.Location = new Point(4, 29);
            tabRecordings.Name = "tabRecordings";
            tabRecordings.Padding = new Padding(3);
            tabRecordings.Size = new Size(750, 396);
            tabRecordings.TabIndex = 1;
            tabRecordings.Text = "Gravações";
            tabRecordings.UseVisualStyleBackColor = true;
            // 
            // btnExportSurgeryDesc
            // 
            btnExportSurgeryDesc.Location = new Point(6, 326);
            btnExportSurgeryDesc.Name = "btnExportSurgeryDesc";
            btnExportSurgeryDesc.Size = new Size(125, 29);
            btnExportSurgeryDesc.TabIndex = 14;
            btnExportSurgeryDesc.Text = "Salvar resultado";
            btnExportSurgeryDesc.TextAlign = ContentAlignment.MiddleLeft;
            btnExportSurgeryDesc.UseVisualStyleBackColor = true;
            btnExportSurgeryDesc.Click += btnExportSurgeryDesc_Click;
            // 
            // btnExportJson
            // 
            btnExportJson.Location = new Point(6, 361);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new Size(125, 29);
            btnExportJson.TabIndex = 13;
            btnExportJson.Text = "Salvar json";
            btnExportJson.TextAlign = ContentAlignment.MiddleLeft;
            btnExportJson.UseVisualStyleBackColor = true;
            btnExportJson.Click += btnExportJson_Click;
            // 
            // numRoom2
            // 
            numRoom2.Location = new Point(6, 26);
            numRoom2.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRoom2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom2.Name = "numRoom2";
            numRoom2.Size = new Size(125, 27);
            numRoom2.TabIndex = 12;
            numRoom2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom2.ValueChanged += numRoom2_ValueChanged;
            // 
            // lblRoom2
            // 
            lblRoom2.AutoSize = true;
            lblRoom2.Location = new Point(3, 3);
            lblRoom2.Name = "lblRoom2";
            lblRoom2.Size = new Size(37, 20);
            lblRoom2.TabIndex = 11;
            lblRoom2.Text = "Sala";
            // 
            // lisView
            // 
            lisView.Columns.AddRange(new ColumnHeader[] { colStart, colDuration, colShots, colPath });
            lisView.FullRowSelect = true;
            lisView.Location = new Point(137, 3);
            lisView.MultiSelect = false;
            lisView.Name = "lisView";
            lisView.ShowGroups = false;
            lisView.Size = new Size(610, 387);
            lisView.TabIndex = 10;
            lisView.UseCompatibleStateImageBehavior = false;
            lisView.View = View.Details;
            lisView.ControlAdded += lisView_ControlAdded;
            // 
            // colStart
            // 
            colStart.Text = "Início";
            // 
            // colDuration
            // 
            colDuration.Text = "Duração";
            // 
            // colShots
            // 
            colShots.Text = "Fotos";
            // 
            // colPath
            // 
            colPath.Text = "Caminho";
            // 
            // dtpDay
            // 
            dtpDay.CustomFormat = "dd/MM/yyyy";
            dtpDay.Format = DateTimePickerFormat.Custom;
            dtpDay.Location = new Point(6, 84);
            dtpDay.Name = "dtpDay";
            dtpDay.Size = new Size(125, 27);
            dtpDay.TabIndex = 1;
            dtpDay.Value = new DateTime(2024, 10, 12, 9, 45, 49, 337);
            dtpDay.ValueChanged += dtpDay_ValueChanged;
            // 
            // lblDay
            // 
            lblDay.AutoSize = true;
            lblDay.Location = new Point(3, 61);
            lblDay.Name = "lblDay";
            lblDay.Size = new Size(32, 20);
            lblDay.TabIndex = 0;
            lblDay.Text = "Dia";
            // 
            // tabOptions
            // 
            tabOptions.Controls.Add(ckbDemo);
            tabOptions.Controls.Add(lblDemo);
            tabOptions.Controls.Add(btnSave);
            tabOptions.Controls.Add(selNeuralNet);
            tabOptions.Controls.Add(lblNeuralNet);
            tabOptions.Controls.Add(numInterval);
            tabOptions.Controls.Add(lblInterval);
            tabOptions.Location = new Point(4, 29);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new Padding(3);
            tabOptions.Size = new Size(750, 396);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Opções";
            tabOptions.UseVisualStyleBackColor = true;
            // 
            // ckbDemo
            // 
            ckbDemo.AutoSize = true;
            ckbDemo.Location = new Point(141, 170);
            ckbDemo.Name = "ckbDemo";
            ckbDemo.Size = new Size(18, 17);
            ckbDemo.TabIndex = 6;
            ckbDemo.UseVisualStyleBackColor = true;
            // 
            // lblDemo
            // 
            lblDemo.AutoSize = true;
            lblDemo.Location = new Point(137, 145);
            lblDemo.Name = "lblDemo";
            lblDemo.Size = new Size(50, 20);
            lblDemo.TabIndex = 5;
            lblDemo.Text = "Demo";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(6, 361);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(125, 29);
            btnSave.TabIndex = 4;
            btnSave.Text = "Salvar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // selNeuralNet
            // 
            selNeuralNet.FormattingEnabled = true;
            selNeuralNet.Items.AddRange(new object[] { "Yolo", "Custom Vision" });
            selNeuralNet.Location = new Point(137, 97);
            selNeuralNet.Name = "selNeuralNet";
            selNeuralNet.Size = new Size(125, 28);
            selNeuralNet.TabIndex = 3;
            // 
            // lblNeuralNet
            // 
            lblNeuralNet.AutoSize = true;
            lblNeuralNet.Location = new Point(137, 74);
            lblNeuralNet.Name = "lblNeuralNet";
            lblNeuralNet.Size = new Size(88, 20);
            lblNeuralNet.TabIndex = 2;
            lblNeuralNet.Text = "Rede neural";
            // 
            // numInterval
            // 
            numInterval.Location = new Point(137, 26);
            numInterval.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(125, 27);
            numInterval.TabIndex = 1;
            numInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Location = new Point(137, 3);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(67, 20);
            lblInterval.TabIndex = 0;
            lblInterval.Text = "Intervalo";
            // 
            // SurgeryWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 453);
            Controls.Add(tab);
            MinimumSize = new Size(800, 500);
            Name = "SurgeryWindow";
            Text = "Form1";
            Load += SurgeryWindow_Load;
            tab.ResumeLayout(false);
            tabRecord.ResumeLayout(false);
            tabRecord.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).EndInit();
            tabRecordings.ResumeLayout(false);
            tabRecordings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom2).EndInit();
            tabOptions.ResumeLayout(false);
            tabOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tab;
        private TabPage tabRecord;
        private TabPage tabRecordings;
        private TabPage tabOptions;
        private Label lblRoom;
        private Button btnStop;
        private Button btnStart;
        private PictureBox imgCapture;
        private NumericUpDown numRoom;
        private DateTimePicker dtpDay;
        private Label lblDay;
        private NumericUpDown numRoom2;
        private Label txtRoom;
        private ListView lisView;
        private Label lblInterval;
        private Label lblNeuralNet;
        private NumericUpDown numInterval;
        private Button btnSave;
        private ComboBox selNeuralNet;
        private Label lblDemo;
        private CheckBox ckbDemo;
        private Label lblRoom2;
        private ColumnHeader colStart;
        private ColumnHeader colDuration;
        private ColumnHeader colShots;
        private Button btnExportSurgeryDesc;
        private Button btnExportJson;
        private ColumnHeader colPath;
        private SaveFileDialog saveFileDialog1;
    }
}
