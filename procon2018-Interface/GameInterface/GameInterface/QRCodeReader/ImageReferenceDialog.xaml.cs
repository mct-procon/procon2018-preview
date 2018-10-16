using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing.Presentation;

namespace GameInterface.QRCodeReader
{
    /// <summary>
    /// ImageReferenceDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ImageReferenceDialog : Window
    {
        new public ImageReferenceViewModel DataContext;

        public ImageReferenceDialog()
        {
            InitializeComponent();
            this.DataContext = new ImageReferenceViewModel();
            base.DataContext = this.DataContext;
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e) => this.Close();
        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.jpg;*.jpeg:*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
                DataContext.LoadImage(ofd.FileName);
            else
                this.Close();
        }

        public static bool ShowDialog(out string ResultText)
        {
            ResultText = null;
            ImageReferenceDialog dig = new ImageReferenceDialog();
            dig.ShowDialog();
            if (dig.DialogResult == true)
            {
                ResultText = dig.DataContext.ResultText;
                return true;
            }
            return false;
        }
    }

    public class ImageReferenceViewModel : ViewModels.ViewModelBase
    {
        private bool isOkButtonEnabled = false;
        public bool IsOkButtonEnabled {
            get => isOkButtonEnabled;
            set => RaisePropertyChanged(ref isOkButtonEnabled, value);
        }

        private string resultText = "";
        public string ResultText {
            get => resultText;
            set => RaisePropertyChanged(ref resultText, value);
        }

        private ImageSource showImage = null;
        public ImageSource ShowImage {
            get => showImage;
            set => RaisePropertyChanged(ref showImage, value);
        }

        public void LoadImage(string filePath)
        {
            BitmapImage image = null;
            try
            {
                image = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }
            catch(Exception ex)
            {
                MessageBox.Show("画像読み込み中にエラーが発生しました．\n" + ex.ToString(), "画像読み込みエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var reader = new BarcodeReader();
            var result = reader.Decode(image);
            if (result == null)
                MessageBox.Show("QRコードを発見できませんでした．", "QRコード読み取りエラー", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                ResultText = result.Text;
                IsOkButtonEnabled = true;
            }
            ShowImage = image;
        }
    }
}
