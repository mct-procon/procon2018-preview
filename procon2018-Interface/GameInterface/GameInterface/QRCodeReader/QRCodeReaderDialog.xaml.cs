using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using OpenCvSharp;
using ZXing;

namespace GameInterface.QRCodeReader
{
    /// <summary>
    /// QRCodeReaderDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class QRCodeReaderDialog : System.Windows.Window
    {
        new private QRCodeReaderViewModel DataContext;

        public QRCodeReaderDialog()
        {
            InitializeComponent();
            DataContext = new QRCodeReaderViewModel(this.Dispatcher);
            base.DataContext = this.DataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) =>
            DataContext.InitMainLoop();

        private void Window_Unloaded(object sender, RoutedEventArgs e) =>
            DataContext.EndMainLoop();

        private void CancelButtonClicked(object sender, RoutedEventArgs e) => this.Close();
        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public static bool ShowDialog(out string ResultText)
        {
            ResultText = null;
            QRCodeReaderDialog dig = new QRCodeReaderDialog();
            dig.ShowDialog();
            if (dig.DialogResult == true) {
                ResultText = dig.DataContext.ResultText;
                return true;
            }
            return false;
        }
    }

    public class QRCodeReaderViewModel : ViewModels.ViewModelBase
    {
        private int currentCameraID = -1;
        public int CurrentCameraID {
            get => currentCameraID;
            set {
                RaisePropertyChanged(ref currentCameraID, value);
                UpdateCameraSource();
            }
        }

        private WriteableBitmap WpfBitmap = null;
        public WriteableBitmap CameraBitmap {
            get => WpfBitmap;
            set => RaisePropertyChanged(ref WpfBitmap, value);
        }

        private string resultText = "";
        public string ResultText {
            get => resultText;
            set => RaisePropertyChanged(ref resultText, value);
        }

        private bool isOkButtonEnabled = false;
        public bool IsOkButtonEnabled {
            get => isOkButtonEnabled;
            set => RaisePropertyChanged(ref isOkButtonEnabled, value);
        }

        private VideoCapture CameraSource = null;

        private ManualResetEventSlim Locker = new ManualResetEventSlim(false);
        private BitmapSourceLuminanceSourceEx QrSource = null;
        private Mat CameraMat = new Mat();
        private BarcodeReader QrReader = new BarcodeReader();

        private Dispatcher WindowDispatcher = null;

        private CancellationTokenSource cancellation;

        public QRCodeReaderViewModel(Dispatcher windowDispatcher) {
            WindowDispatcher = windowDispatcher;
        }

        public void UpdateCameraSource()
        {
            if(CurrentCameraID < 0)
            {
                CameraSource = null;
                return;
            }
            Locker.Reset();
            CameraSource = new VideoCapture(CurrentCameraID);
            if(!CameraSource.IsOpened())
            {
                CameraSource = null;
                return;
            }
            QrSource = new BitmapSourceLuminanceSourceEx(CameraSource.FrameWidth, CameraSource.FrameHeight);
            WpfBitmap = new WriteableBitmap(CameraSource.FrameWidth, CameraSource.FrameHeight, 96, 96, PixelFormats.Bgr24, null);
            Locker.Set();
        }

        public void InitMainLoop()
        {
            cancellation?.Dispose();
            cancellation = new CancellationTokenSource();
            UpdateCameraSource();
            Task.Run((Action)MainLoop, cancellation.Token);
        }

        public void MainLoop()
        {
            while(true)
            {
                Thread.Sleep(1000 / 30);
                Locker.Wait();
                if (cancellation.IsCancellationRequested)
                    return;
                if (CameraSource == null)
                    continue;
                if(CameraSource.Read(CameraMat))
                {
                    WindowDispatcher.Invoke(() =>
                    {
                        OpenCvSharp.Extensions.WriteableBitmapConverter.ToWriteableBitmap(CameraMat, WpfBitmap);
                        RaisePropertyChanged(nameof(CameraBitmap));
                        QrSource.UpdateImage(WpfBitmap);
                    });
                    var QrResult = QrReader.Decode(QrSource);

                    if (QrResult != null)
                    {
                        WindowDispatcher.Invoke(() =>
                        {
                            ResultText = QrResult.Text;
                        });
                    }
                }
            }
        }

        public void EndMainLoop()
        {
            Locker.Reset();
            cancellation.Cancel();
            Locker.Set();
            CameraMat.Dispose();
            CameraSource.Dispose();
        }
    }
}
