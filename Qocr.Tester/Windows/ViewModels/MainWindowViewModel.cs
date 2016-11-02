using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;

using Qocr.Core.Approximation;
using Qocr.Core.Data.Serialization;
using Qocr.Generator;
using Qocr.Generator.Data;
using Qocr.Tester.Helpers;

namespace Qocr.Tester.Windows.ViewModels
{
    public class MainWindowViewModel : NotificationObject
    {
        private ImageSource _approximatedImage;

        private ImageSource _sourceImage;

        private EulerGenerator _generator = new EulerGenerator();

        private bool _genStated;

        //private void TestGen()
        //{
        //    using (FileStream fileStream = new FileStream("Gen.bin", FileMode.Open))
        //    {
        //        var container = CompressionUtils.Decompress<EulerContainer>(fileStream);

        //    }
        //}

        private bool _isPastProcessing;

        private ImageSource _currentGenImage;

        private Font _currentFont;

        public MainWindowViewModel()
        {
            OpenSourceImageCommand = new DelegateCommand(OpenSourceImage);
            ImagePastCommand = new DelegateCommand(ImagePast);
            GenCommand = new DelegateCommand(GenStart, CanGen);
            _generator.BitmapCreated += GeneratorOnBitmapCreated;
        }

        public Font CurrentFont
        {
            get
            {
                return _currentFont;
            }
            set
            {
                if (Equals(value, _currentFont))
                {
                    return;
                }

                _currentFont?.Dispose();
                _currentFont = value;
                RaisePropertyChanged(nameof(CurrentFont));
            }
        }

        public ImageSource CurrentGenImage
        {
            get
            {
                return _currentGenImage;
            }

            set
            {
                _currentGenImage = value;
                RaisePropertyChanged(() => CurrentGenImage);
            }
        }

        public ImageSource SourceImage
        {
            get
            {
                return _sourceImage;
            }
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
            get
            {
                return _approximatedImage;
            }
            set
            {
                _approximatedImage = value;
                RaisePropertyChanged(() => ApproximatedImage);
            }
        }

        public DelegateCommand GenCommand { get; private set; }

        public DelegateCommand ImagePastCommand { get; private set; }

        public DelegateCommand OpenSourceImageCommand { get; private set; }

        private bool CanGen()
        {
            return !_genStated;
        }

        private async void GenStart()
        {
            _genStated = true;
            GenCommand.RaiseCanExecuteChanged();
            _genImageNumber = 0;

            if (Directory.Exists("BmpDebug"))
            {
                Directory.Delete("BmpDebug", true);
            }

            Directory.CreateDirectory("BmpDebug");
            while (!Directory.Exists("BmpDebug"));

            DateTime dNow = DateTime.Now;

            EulerContainer container = new EulerContainer();
            int minFont = 8, maxFont = 35;

            //int minFont = 11, maxFont = 11;

            var ruLang = await GenerateLanguage("RU-ru", minFont, maxFont, 'а', 'я');
            var enLang = await GenerateLanguage("EN-en", minFont, maxFont, 'a', 'z');

            container.Languages.Add(ruLang);
            container.Languages.Add(enLang);

            var compression = CompressionUtils.Compress(container);
            using (FileStream fileStream = new FileStream("Gen.bin", FileMode.Create))
            {
                compression.Position = 0;
                compression.CopyTo(fileStream);
            }

            CurrentGenImage = null;
            MessageBox.Show($"Время создания {DateTime.Now - dNow}");

            _genStated = false;
            GenCommand.RaiseCanExecuteChanged();
        }

        private async Task<Language> GenerateLanguage(
            string localization,
            int minFont,
            int maxFont,
            char startChr,
            char endChr)
        {
            List<char> chars = new List<char>();
            for (char c = startChr; c <= endChr; c++)
            {
                chars.Add(c);
            }

            var fontFamilies = new[]
            {
                new System.Drawing.FontFamily("Times New Roman"),
                new System.Drawing.FontFamily("Arial"),
                new System.Drawing.FontFamily("Courier"),
            };

            return await _generator.GenerateLanguage(chars.ToArray(), minFont, maxFont, localization, fontFamilies);
        }

        private static ImageSource IconToImageSource(Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        private int _genImageNumber = 0;
        private void GeneratorOnBitmapCreated(object sender, BitmapEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(
                new Action(
                    () =>
                    {
                        var chr = args.Chr;
                        var bmp = args.GeneratedBitmap;
                        var font = args.CurrentFont;
                        var fontName = font.FontFamily.Name;
                        var fontSize = font.Size;
                        var fontStyle = font.Style;
                        var debugDir = "BmpDebug";

                        var chrEx = chr.ToString();
                        if (char.IsLetter(chr))
                        {
                            chrEx = char.IsUpper(chr) ? $"^{chr}" : $"{chr}";
                        }

                        var chrDir = Path.Combine(debugDir, chrEx);
                        if (!Directory.Exists(chrDir))
                        {
                            Directory.CreateDirectory(chrDir);
                            while (!Directory.Exists(chrDir));
                        }

                        var debugFileName =
                            $"{fontName} {fontSize} {fontStyle}.png";
                        bmp.Save(Path.Combine(chrDir, debugFileName), ImageFormat.Png);

                        try
                        {
                            CurrentGenImage = BitmapUtils.SourceFromBitmap(bmp);
                        }
                        catch (Exception)
                        {
                            CurrentGenImage = IconToImageSource(SystemIcons.Error);
                        }
                        

                        _genImageNumber ++;
                        bmp.Dispose();
                        CurrentFont = args.CurrentFont;
                    }));
        }

        private void ImagePast()
        {
            if (_isPastProcessing)
            {
                return;
            }

            _isPastProcessing = true;

            var clipboardImage = Clipboard.GetImage();
            if (clipboardImage == null)
            {
                MessageBox.Show("Image clipboard is empty");
                return;
            }

            SourceImage = clipboardImage;

            _isPastProcessing = false;
        }

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
                var bitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);

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