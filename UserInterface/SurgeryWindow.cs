using FileManagementService;
using PhotographService;
using static UserInterface.Enums;

namespace UserInterface
{
    public partial class SurgeryWindow : Form
    {
        private uint _room = 0;
        private readonly Photographer _photographer;
        private string _tempPath;

        public ApplicationState State { get; private set; }

        public SurgeryWindow()
        {
            InitializeComponent();
            _photographer = new Photographer();
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
            DateTimeOffset start = DateTimeOffset.Now;
            _tempPath = Supervisor.CreateTemporaryFolder(room, start);

            _photographer.StartCapture(_tempPath, 5, ShowImage);
        }

        private void StopRecording()
        {
            ToggleState(ApplicationState.Stopped);
            _photographer.StopCapture();
            Supervisor.MoveTemporaryFolder(_tempPath);
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
    }
}
