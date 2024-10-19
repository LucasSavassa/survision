using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using Comuns.Services;
using FileManagementService;
using PhotographService;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using UserInterface.Enums;
using UserInterface.Properties;
using YoloPredictionService;

namespace UserInterface
{
    public partial class SurgeryWindow : Form
    {
        private uint _room = 0;
        private Photographer? _photographer;
        private string _tempPath = string.Empty;
        private bool _isRecording = false;

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
            numInferiorThreshold.Value = Settings.Default.InferiorThreshold;
            numSuperiorThreshold.Value = Settings.Default.SuperiorThreshold;
            Supervisor.InferiorThreshold = Settings.Default.InferiorThreshold;
            numSuperiorThreshold.Value = Settings.Default.SuperiorThreshold;
        }

        public void btnStart_Click(object sender, EventArgs e)
        {
            if (!RoomIsValid())
            {
                DisplayValidationMessage(Constantes.InvalidRoomMessage);
                return;
            }
            
            StartRecording();
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

        private void DisplayValidationMessage(string message)
        {
            MessageBox.Show(message);
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
                    _isRecording = true;
                    break;
                case ApplicationState.Stopped:
                default:
                    numRoom.Enabled = true;
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    _isRecording = false;
                    break;
            }
        }

        private void ShowImage(Image image)
        {
            Bitmap bitmap = new Bitmap(image);

            if(Settings.Default.IsDemo)
            {
                IPredictionResult predictionResult = Supervisor.GetPredictionResult(bitmap);
                DrawService.DrawDetectedObjects(bitmap, predictionResult, (double)numInferiorThreshold.Value);
            }
            
            imgCapture.Image = bitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_isRecording)
            {
                MessageBox.Show(Constantes.IsRecordingMessage);
                return;
            }

            Settings.Default.Interval = (int)numInterval.Value;
            Settings.Default.NeuralNet = selNeuralNet.SelectedIndex;
            Settings.Default.IsDemo = ckbDemo.Checked;
            Settings.Default.SuperiorThreshold = (int)numSuperiorThreshold.Value;
            Settings.Default.InferiorThreshold = (int)numInferiorThreshold.Value;
            Settings.Default.Save();

            Supervisor.NeuralNetwork = (NeuralNetworkType)selNeuralNet.SelectedIndex;
            Supervisor.InferiorThreshold = (int)numInferiorThreshold.Value;
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
                DisplayValidationMessage(Constantes.InvalidRoomMessage);
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
                    string content = string.Empty;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }

                    if (string.IsNullOrEmpty(content))
                    {
                        MessageBox.Show("Não foi possível exportar a descrição cirúrgica porque o arquivo de resultados está vazio.");
                        return;
                    }

                    SurgeryResult? surgeryResult = JsonSerializer.Deserialize<SurgeryResult>(content);

                    if (surgeryResult is null)
                    {
                        MessageBox.Show("Não foi possível exportar a descrição cirúrgica porque o arquivo de resultados não é válido.");
                        return;
                    }

                    string summary = SummarizeSurgeryResult(surgeryResult);

                    saveFileDialog1.Filter = "Text files (*.txt)|*.txt";
                    saveFileDialog1.FileName = Path.GetFileName(results.FullName);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Stream output = saveFileDialog1.OpenFile())
                        {
                            using (StreamWriter writer = new StreamWriter(output))
                            {
                                writer.Write(summary);
                            }
                        }
                    }
                }
            }
        }

        private string SummarizeSurgeryResult(SurgeryResult surgeryResult)
        {
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Descrição da cirurgia");
            summary.AppendLine($"Sala: {surgeryResult.Surgery.Room}");
            summary.AppendLine($"Início: {surgeryResult.Surgery.Start}");
            summary.AppendLine($"Duração: {surgeryResult.Surgery.Seconds} segundos");
            summary.AppendLine($"Fotos: {surgeryResult.Surgery.Shots}");
            summary.AppendLine("");
            summary.AppendLine("Materiais detectados no início:");

            PictureResult firstPicture = surgeryResult.Timeline.First();
            IEnumerable<(string, int)> firstGrouping = new List<(string, int)>();
            if (firstPicture is not null)
            {
                ICollection<Prediction> detections = firstPicture.Detections;
                if (detections.Count() > 0)
                {
                    firstGrouping = detections.GroupBy(detection => detection.Name).Select(group => (group.First().Name, group.Count()));
                    foreach ((string name, int count) in firstGrouping)
                    {
                        summary.AppendLine($"{name}: ({count})");
                    }
                }
            }

            summary.AppendLine("");
            summary.AppendLine("Materiais detectados no final:");
            PictureResult lastPicture = surgeryResult.Timeline.Last();
            IEnumerable<(string, int)> lastGrouping = new List<(string, int)>();
            if (lastPicture is not null)
            {
                ICollection<Prediction> detections = lastPicture.Detections;
                if (detections.Count() > 0)
                {
                    lastGrouping = detections.GroupBy(detection => detection.Name).Select(group => (group.First().Name, group.Count()));
                    foreach ((string name, int count) in lastGrouping)
                    {
                        summary.AppendLine($"{name}: ({count})");
                    }
                }
            }

            summary.AppendLine("");
            summary.AppendLine("Diferença:");
            IEnumerable<(string, int)> delta = CalculateDelta(firstGrouping, lastGrouping);
            foreach ((string name, int count) in delta)
            {
                if (count < 0)
                {
                    summary.AppendLine($"{count} {name} adicionados na bandeja.");
                }
                else
                {
                    summary.AppendLine($"{count} {name} removidos da bandeja.");
                }
            }

            return summary.ToString();
        }

        private IEnumerable<(string, int)> CalculateDelta(IEnumerable<(string, int)> firstGrouping, IEnumerable<(string, int)> lastGrouping)
        {
            foreach ((string name, int count) in firstGrouping)
            {
                int delta = lastGrouping.FirstOrDefault(x => x.Item1 == name).Item2 - count;
                yield return (name, delta);
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
