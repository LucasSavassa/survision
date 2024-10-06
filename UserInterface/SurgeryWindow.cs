using FileManagementService;
using FileManagementService.Enums;
using PhotographService;
using UserInterface.Enums;
using UserInterface.Properties;

namespace UserInterface
{
    public partial class SurgeryWindow : Form
    {
        private uint _room = 0;
        private Photographer _photographer;
        private string _tempPath;

        public ApplicationState State { get; private set; }

        public SurgeryWindow()
        {
            InitializeComponent();
        }

        private void SurgeryWindow_Load(object sender, EventArgs e)
        {
            _photographer = new Photographer();
            ApplySettings();
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

        public void btnStop_Click(object sender, EventArgs e)
        {
            StopRecording();
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
    }
}
