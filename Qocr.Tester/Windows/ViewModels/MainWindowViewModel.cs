using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using Qocr.Core.Approximation;
using Qocr.Tester.Helpers;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace Qocr.Tester.Windows.ViewModels
{
    public class MainWindowViewModel : NotificationObject
    {
        private ImageSource _approximatedImage;

        private ImageSource _sourceImage;

        public MainWindowViewModel()
        {
            OpenSourceImageCommand = new DelegateCommand(OpenSourceImage);
            ImagePastCommand = new DelegateCommand(ImagePast);
        }

        private void ImagePast()
        {
            var clipboardImage = Clipboard.GetImage();
            if (clipboardImage == null)
            {
                MessageBox.Show("Image clipboard is empty");
                return;
            }

            SourceImage = clipboardImage;
        }

        public ImageSource SourceImage
        {
            get { return _sourceImage; }
            set
            {
                _sourceImage = value;

                if (value != null)
                {

                    //MyApproximator appx = new MyApproximator(); 
                       OneBitApproximator appx = new OneBitApproximator(); 
                       // BrightnessBaseApproximator appx = new BrightnessBaseApproximator();
                       //FastApproximator appx = new FastApproximator();

                       var resultImage = appx.Approximate(BitmapUtils.BitmapFromSource((BitmapSource)value));
                    ApproximatedImage = BitmapUtils.SourceFromBitmap(resultImage.ToBitmap());
                }
                else
                {
                    ApproximatedImage = null;
                }

                RaisePropertyChanged(() => SourceImage);
            }
        }

        public ImageSource ApproximatedImage
        {
            get { return _approximatedImage; }
            set
            {
                _approximatedImage = value;
                RaisePropertyChanged(() => ApproximatedImage);
            }
        }

        public DelegateCommand ImagePastCommand { get; private set; }
        
        public DelegateCommand OpenSourceImageCommand { get; private set; }

        private void OpenSourceImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF,*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Multiselect = false
            };

            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var bitmap = (Bitmap) Image.FromFile(openFileDialog.FileName);
                
                BitmapSource sourceImage = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
                
                SourceImage = sourceImage;
            }
        }
    }
}