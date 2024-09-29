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
            tabOptions = new TabPage();
            tab.SuspendLayout();
            tabRecord.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).BeginInit();
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
            tabRecordings.Location = new Point(4, 29);
            tabRecordings.Name = "tabRecordings";
            tabRecordings.Padding = new Padding(3);
            tabRecordings.Size = new Size(750, 396);
            tabRecordings.TabIndex = 1;
            tabRecordings.Text = "Gravações";
            tabRecordings.UseVisualStyleBackColor = true;
            // 
            // tabOptions
            // 
            tabOptions.Location = new Point(4, 29);
            tabOptions.Name = "tabOptions";
            tabOptions.Padding = new Padding(3);
            tabOptions.Size = new Size(750, 396);
            tabOptions.TabIndex = 2;
            tabOptions.Text = "Opções";
            tabOptions.UseVisualStyleBackColor = true;
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
            tab.ResumeLayout(false);
            tabRecord.ResumeLayout(false);
            tabRecord.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRoom).EndInit();
            ((System.ComponentModel.ISupportInitialize)imgCapture).EndInit();
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
    }
}
