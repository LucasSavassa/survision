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
                _photographer.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SurgeryWindow));
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
            numInferiorThreshold = new NumericUpDown();
            lblInferiorThreshold = new Label();
            numSuperiorThreshold = new NumericUpDown();
            lblSuperiorThreshold = new Label();
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
            ((System.ComponentModel.ISupportInitialize)numInferiorThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSuperiorThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            SuspendLayout();
            // 
            // tab
            // 
            tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tab.Controls.Add(tabRecord);
            tab.Controls.Add(tabRecordings);
            tab.Controls.Add(tabOptions);
            tab.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tab.Location = new Point(10, 9);
            tab.Margin = new Padding(3, 2, 3, 2);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(1144, 671);
            tab.TabIndex = 0;
            // 
            // tabRecord
            // 
            tabRecord.Controls.Add(numRoom);
            tabRecord.Controls.Add(imgCapture);
            tabRecord.Controls.Add(btnStop);
            tabRecord.Controls.Add(btnStart);
            tabRecord.Controls.Add(lblRoom);
            tabRecord.Location = new Point(4, 30);
            tabRecord.Margin = new Padding(3, 2, 3, 2);
            tabRecord.Name = "tabRecord";
            tabRecord.Padding = new Padding(3, 2, 3, 2);
            tabRecord.Size = new Size(1136, 637);
            tabRecord.TabIndex = 0;
            tabRecord.Text = "Gravar";
            tabRecord.UseVisualStyleBackColor = true;
            // 
            // numRoom
            // 
            numRoom.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numRoom.Location = new Point(5, 36);
            numRoom.Margin = new Padding(3, 2, 3, 2);
            numRoom.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRoom.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom.Name = "numRoom";
            numRoom.Size = new Size(150, 35);
            numRoom.TabIndex = 5;
            numRoom.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // imgCapture
            // 
            imgCapture.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imgCapture.BackColor = Color.DarkGray;
            imgCapture.Location = new Point(161, 4);
            imgCapture.Margin = new Padding(3, 2, 3, 2);
            imgCapture.Name = "imgCapture";
            imgCapture.Size = new Size(971, 631);
            imgCapture.SizeMode = PictureBoxSizeMode.Zoom;
            imgCapture.TabIndex = 4;
            imgCapture.TabStop = false;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.Enabled = false;
            btnStop.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStop.Location = new Point(5, 587);
            btnStop.Margin = new Padding(3, 2, 3, 2);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(150, 46);
            btnStop.TabIndex = 3;
            btnStop.Text = "Parar";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStart.BackColor = Color.Transparent;
            btnStart.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStart.Location = new Point(5, 538);
            btnStart.Margin = new Padding(3, 2, 3, 2);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(150, 45);
            btnStart.TabIndex = 2;
            btnStart.Text = "Iniciar";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblRoom.Location = new Point(0, 2);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(51, 30);
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
            tabRecordings.Location = new Point(4, 30);
            tabRecordings.Margin = new Padding(3, 2, 3, 2);
            tabRecordings.Name = "tabRecordings";
            tabRecordings.Padding = new Padding(3, 2, 3, 2);
            tabRecordings.Size = new Size(1136, 637);
            tabRecordings.TabIndex = 1;
            tabRecordings.Text = "Gravações";
            tabRecordings.UseVisualStyleBackColor = true;
            // 
            // btnExportSurgeryDesc
            // 
            btnExportSurgeryDesc.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportSurgeryDesc.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnExportSurgeryDesc.Location = new Point(6, 539);
            btnExportSurgeryDesc.Margin = new Padding(3, 2, 3, 2);
            btnExportSurgeryDesc.Name = "btnExportSurgeryDesc";
            btnExportSurgeryDesc.Size = new Size(149, 45);
            btnExportSurgeryDesc.TabIndex = 14;
            btnExportSurgeryDesc.Text = "Dados resumidos";
            btnExportSurgeryDesc.TextAlign = ContentAlignment.MiddleLeft;
            btnExportSurgeryDesc.UseVisualStyleBackColor = true;
            btnExportSurgeryDesc.Click += btnExportSurgeryDesc_Click;
            // 
            // btnExportJson
            // 
            btnExportJson.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportJson.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnExportJson.Location = new Point(5, 588);
            btnExportJson.Margin = new Padding(3, 2, 3, 2);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new Size(150, 45);
            btnExportJson.TabIndex = 13;
            btnExportJson.Text = "Dados completos";
            btnExportJson.TextAlign = ContentAlignment.MiddleLeft;
            btnExportJson.UseVisualStyleBackColor = true;
            btnExportJson.Click += btnExportJson_Click;
            // 
            // numRoom2
            // 
            numRoom2.Font = new Font("Segoe UI", 15.75F);
            numRoom2.Location = new Point(5, 34);
            numRoom2.Margin = new Padding(3, 2, 3, 2);
            numRoom2.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            numRoom2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom2.Name = "numRoom2";
            numRoom2.Size = new Size(150, 35);
            numRoom2.TabIndex = 12;
            numRoom2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numRoom2.ValueChanged += numRoom2_ValueChanged;
            // 
            // lblRoom2
            // 
            lblRoom2.AutoSize = true;
            lblRoom2.Font = new Font("Segoe UI", 15.75F);
            lblRoom2.Location = new Point(3, 2);
            lblRoom2.Name = "lblRoom2";
            lblRoom2.Size = new Size(51, 30);
            lblRoom2.TabIndex = 11;
            lblRoom2.Text = "Sala";
            // 
            // lisView
            // 
            lisView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lisView.Columns.AddRange(new ColumnHeader[] { colStart, colDuration, colShots, colPath });
            lisView.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lisView.FullRowSelect = true;
            lisView.Location = new Point(161, 2);
            lisView.Margin = new Padding(3, 2, 3, 2);
            lisView.MultiSelect = false;
            lisView.Name = "lisView";
            lisView.ShowGroups = false;
            lisView.Size = new Size(974, 634);
            lisView.TabIndex = 10;
            lisView.UseCompatibleStateImageBehavior = false;
            lisView.View = View.Details;
            lisView.ControlAdded += lisView_ControlAdded;
            // 
            // colStart
            // 
            colStart.Text = "Início";
            colStart.Width = 120;
            // 
            // colDuration
            // 
            colDuration.Text = "Duração";
            colDuration.Width = 120;
            // 
            // colShots
            // 
            colShots.Text = "Fotos";
            colShots.Width = 120;
            // 
            // colPath
            // 
            colPath.Text = "Caminho";
            colPath.Width = 360;
            // 
            // dtpDay
            // 
            dtpDay.CustomFormat = "dd/MM/yyyy";
            dtpDay.Font = new Font("Segoe UI", 15.75F);
            dtpDay.Format = DateTimePickerFormat.Custom;
            dtpDay.Location = new Point(5, 115);
            dtpDay.Margin = new Padding(3, 2, 3, 2);
            dtpDay.Name = "dtpDay";
            dtpDay.Size = new Size(150, 35);
            dtpDay.TabIndex = 1;
            dtpDay.Value = new DateTime(2024, 12, 2, 0, 0, 0, 0);
            dtpDay.ValueChanged += dtpDay_ValueChanged;
            // 
            // lblDay
            // 
            lblDay.AutoSize = true;
            lblDay.Font = new Font("Segoe UI", 15.75F);
            lblDay.Location = new Point(0, 83);
            lblDay.Name = "lblDay";
            lblDay.Size = new Size(44, 30);
            lblDay.TabIndex = 0;
            lblDay.Text = "Dia";
            // 
            // tabOptions
            // 
            tabOptions.Controls.Add(numInferiorThreshold);
            tabOptions.Controls.Add(lblInferiorThreshold);
            tabOptions.Controls.Add(numSuperiorThreshold);
            tabOptions.Controls.Add(lblSuperiorThreshold);
            tabOptions.Controls.Add(ckbDemo);
            tabOptions.Controls.Add(lblDemo);
            tabOptions.Controls.Add(btnSave);
            tabOptions.Controls.Add(selNeuralNet);
            tabOptions.Controls.Add(lblNeuralNet);
            tabOptions.Controls.Add(numInterval);
            tabOptions.Controls.Add(lblInterval);
            tabOptions.Location = new Point(4, 30);
            tabOptions.Margin = new Padding(3, 2, 3, 2);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new Padding(3, 2, 3, 2);
            tabOptions.Size = new Size(1136, 637);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Opções";
            tabOptions.UseVisualStyleBackColor = true;
            // 
            // numInferiorThreshold
            // 
            numInferiorThreshold.Font = new Font("Segoe UI", 15.75F);
            numInferiorThreshold.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numInferiorThreshold.Location = new Point(6, 278);
            numInferiorThreshold.Margin = new Padding(3, 2, 3, 2);
            numInferiorThreshold.Maximum = new decimal(new int[] { 75, 0, 0, 0 });
            numInferiorThreshold.Name = "numInferiorThreshold";
            numInferiorThreshold.Size = new Size(200, 35);
            numInferiorThreshold.TabIndex = 10;
            numInferiorThreshold.Value = new decimal(new int[] { 75, 0, 0, 0 });
            // 
            // lblInferiorThreshold
            // 
            lblInferiorThreshold.AutoSize = true;
            lblInferiorThreshold.Font = new Font("Segoe UI", 15.75F);
            lblInferiorThreshold.Location = new Point(6, 246);
            lblInferiorThreshold.Name = "lblInferiorThreshold";
            lblInferiorThreshold.Size = new Size(141, 30);
            lblInferiorThreshold.TabIndex = 9;
            lblInferiorThreshold.Text = "Limite inferior";
            // 
            // numSuperiorThreshold
            // 
            numSuperiorThreshold.Font = new Font("Segoe UI", 15.75F);
            numSuperiorThreshold.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numSuperiorThreshold.Location = new Point(6, 193);
            numSuperiorThreshold.Margin = new Padding(3, 2, 3, 2);
            numSuperiorThreshold.Minimum = new decimal(new int[] { 75, 0, 0, 0 });
            numSuperiorThreshold.Name = "numSuperiorThreshold";
            numSuperiorThreshold.Size = new Size(200, 35);
            numSuperiorThreshold.TabIndex = 8;
            numSuperiorThreshold.Value = new decimal(new int[] { 85, 0, 0, 0 });
            // 
            // lblSuperiorThreshold
            // 
            lblSuperiorThreshold.AutoSize = true;
            lblSuperiorThreshold.Font = new Font("Segoe UI", 15.75F);
            lblSuperiorThreshold.Location = new Point(6, 161);
            lblSuperiorThreshold.Name = "lblSuperiorThreshold";
            lblSuperiorThreshold.Size = new Size(150, 30);
            lblSuperiorThreshold.TabIndex = 7;
            lblSuperiorThreshold.Text = "Limite superior";
            // 
            // ckbDemo
            // 
            ckbDemo.AutoSize = true;
            ckbDemo.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ckbDemo.Location = new Point(6, 362);
            ckbDemo.Margin = new Padding(3, 2, 3, 2);
            ckbDemo.Name = "ckbDemo";
            ckbDemo.Size = new Size(15, 14);
            ckbDemo.TabIndex = 6;
            ckbDemo.UseVisualStyleBackColor = true;
            // 
            // lblDemo
            // 
            lblDemo.AutoSize = true;
            lblDemo.Font = new Font("Segoe UI", 15.75F);
            lblDemo.Location = new Point(3, 330);
            lblDemo.Name = "lblDemo";
            lblDemo.Size = new Size(69, 30);
            lblDemo.TabIndex = 5;
            lblDemo.Text = "Demo";
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.Font = new Font("Segoe UI", 15.75F);
            btnSave.Location = new Point(6, 588);
            btnSave.Margin = new Padding(3, 2, 3, 2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(200, 45);
            btnSave.TabIndex = 4;
            btnSave.Text = "Salvar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // selNeuralNet
            // 
            selNeuralNet.Font = new Font("Segoe UI", 15.75F);
            selNeuralNet.FormattingEnabled = true;
            selNeuralNet.Items.AddRange(new object[] { "Yolo", "Custom Vision" });
            selNeuralNet.Location = new Point(6, 112);
            selNeuralNet.Margin = new Padding(3, 2, 3, 2);
            selNeuralNet.Name = "selNeuralNet";
            selNeuralNet.Size = new Size(200, 38);
            selNeuralNet.TabIndex = 3;
            // 
            // lblNeuralNet
            // 
            lblNeuralNet.AutoSize = true;
            lblNeuralNet.Font = new Font("Segoe UI", 15.75F);
            lblNeuralNet.Location = new Point(6, 80);
            lblNeuralNet.Name = "lblNeuralNet";
            lblNeuralNet.Size = new Size(123, 30);
            lblNeuralNet.TabIndex = 2;
            lblNeuralNet.Text = "Rede neural";
            // 
            // numInterval
            // 
            numInterval.Font = new Font("Segoe UI", 15.75F);
            numInterval.Location = new Point(6, 34);
            numInterval.Margin = new Padding(3, 2, 3, 2);
            numInterval.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(200, 35);
            numInterval.TabIndex = 1;
            numInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Font = new Font("Segoe UI", 15.75F);
            lblInterval.Location = new Point(6, 2);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(94, 30);
            lblInterval.TabIndex = 0;
            lblInterval.Text = "Intervalo";
            // 
            // SurgeryWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1167, 695);
            Controls.Add(tab);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(702, 385);
            Name = "SurgeryWindow";
            Text = "Survision";
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
            ((System.ComponentModel.ISupportInitialize)numInferiorThreshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSuperiorThreshold).EndInit();
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
        private NumericUpDown numInferiorThreshold;
        private Label lblInferiorThreshold;
        private NumericUpDown numSuperiorThreshold;
        private Label lblSuperiorThreshold;
    }
}
