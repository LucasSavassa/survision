using Comuns.Classes;
using FileManagementService;
using FileManagementService.Enums;
using PhotographService;
using System.IO.Compression;
using System.Text.Json;
using UserInterface.Enums;
using UserInterface.Properties;

namespace UserInterface
{
    public partial class SurgeryWindow : Form
    {
        private uint _room = 0;
        private Photographer? _photographer;
        private string _tempPath = string.Empty;

        public ApplicationState State { get; private set; }

        public SurgeryWindow()
        {
            InitializeComponent();
        }

        private void SurgeryWindow_Load(object sender, EventArgs e)
        {
            _photographer = new Photographer();
            ApplySettings();
            UpdateListView();
        }

        private void ApplySettings()
        {
            numInterval.Value = Settings.Default.Interval;

            selNeuralNet.SelectedIndex = Settings.Default.NeuralNet;
            Supervisor.NeuralNetwork = (NeuralNetworkType)Settings.Default.NeuralNet;

            ckbDemo.Checked = Settings.Default.IsDemo;
        }

        public void btnStart_Click(object sender, EventArgs e)
        {
            if (!RoomIsValid())
            {
                DisplayValidationMessage();
            }
            else
            {
                StartRecording();
            }
        }

        public async void btnStop_Click(object sender, EventArgs e)
        {
            await StopRecording();
        }

        private bool RoomIsValid()
        {
            decimal value = numRoom.Value;
            if (value.Scale > 0) return false;
            return true;
        }

        private void DisplayValidationMessage()
        {
            MessageBox.Show(Constantes.InvalidRoomMessage);
        }

        private void StartRecording()
        {
            ToggleState(ApplicationState.Recording);

            int room = (int)numRoom.Value;
            DateTime start = DateTime.Now;
            int interval = (int)numInterval.Value;
            _tempPath = Supervisor.CreateTemporaryFolder(room, start);

            _photographer.StartCapture(_tempPath, interval, ShowImage);
        }

        private async Task StopRecording()
        {
            SubscribeToEvents();
            ToggleState(ApplicationState.Stopped);
            _photographer.StopCapture();
            await _photographer.WaitEnd();
            RecordingMetadata metadata = _photographer.Metadata;
            string metadataPath = Path.Combine(_tempPath, "metadata.json");
            Supervisor.UpdateMetadata(metadataPath, metadata.Shots, metadata.Seconds);
            string newPath = Supervisor.ZipFolder(_tempPath);
            Supervisor.MoveToQueue(newPath);
        }

        private void ToggleState(ApplicationState state)
        {
            State = state;

            switch (state)
            {
                case ApplicationState.Recording:
                    numRoom.Enabled = false;
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    break;
                case ApplicationState.Stopped:
                default:
                    numRoom.Enabled = true;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    break;
            }
        }

        private void ShowImage(Image image)
        {
            imgCapture.Image = new Bitmap(image);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.Interval = (int)numInterval.Value;
            Settings.Default.NeuralNet = selNeuralNet.SelectedIndex;
            Settings.Default.IsDemo = ckbDemo.Checked;
            Settings.Default.Save();

            Supervisor.NeuralNetwork = (NeuralNetworkType)selNeuralNet.SelectedIndex;
        }

        private void numRoom2_ValueChanged(object sender, EventArgs e)
        {
            UpdateListView();
        }

        private void dtpDay_ValueChanged(object sender, EventArgs e)
        {
            UpdateListView();
        }

        private void lisView_ControlAdded(object sender, ControlEventArgs e)
        {
            UpdateListView();
        }

        private void UpdateListView()
        {
            if (!RoomIsValid())
            {
                DisplayValidationMessage();
            }

            lisView.Items.Clear();

            int room = (int)numRoom2.Value;
            DateTime date = dtpDay.Value.Date;
            IEnumerable<string> entries = Supervisor.GetSurgeriesAtGallery(room, date);

            foreach (string entry in entries)
            {
                using (ZipArchive zip = ZipFile.OpenRead(entry))
                {
                    ZipArchiveEntry metadata = zip.Entries.First(x => x.Name == "metadata.json");
                    using (Stream stream = metadata.Open())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string json = reader.ReadToEnd();
                            Surgery? surgery = JsonSerializer.Deserialize<Surgery>(json);
                            if (surgery is not null)
                            {
                                ListViewItem item = new ListViewItem();
                                item.Text = $"{surgery.Start}";
                                item.SubItems.Add($"{surgery.Seconds}");
                                item.SubItems.Add($"{surgery.Shots}");
                                item.SubItems.Add(entry);
                                lisView.Items.Add(item);
                            }
                        }
                    }
                }
            }

            lisView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void btnExportSurgeryDesc_Click(object sender, EventArgs e)
        {
            if (lisView.SelectedItems.Count < 1)
            {
                MessageBox.Show("Não foi possível exportar a descrição cirúrgica porque nenhuma cirurgia foi selecionada.");
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            if (lisView.SelectedItems.Count < 1)
            {
                MessageBox.Show("Não foi possível exportar o JSON porque nenhuma cirurgia foi selecionada.");
                return;
            }

            string zipPath = lisView.SelectedItems[0].SubItems[3].Text;
            if (!File.Exists(zipPath))
            {
                MessageBox.Show("Não foi possível encontrar a pasta da cirurgia selecionada.");
                return;
            }

            using (ZipArchive zip = ZipFile.OpenRead(zipPath))
            {
                ZipArchiveEntry? results = zip.GetEntry("results.json");

                if (results is null)
                {
                    MessageBox.Show("Não foi possível encontrar o arquivo de resultados da cirurgia selecionada.");
                    return;
                }

                using (Stream stream = results.Open())
                {
                    saveFileDialog1.Filter = "JSON files (*.json)|*.json";
                    saveFileDialog1.FileName = Path.GetFileName(results.FullName);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Stream output = saveFileDialog1.OpenFile())
                        {
                            stream.CopyTo(output);
                        }
                    }
                }
            }
        }
    }
}
