using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
using Qocr.Core.Interfaces;
using Qocr.Core.Recognition;
using Qocr.Core.Utils;
using Qocr.Generator;
using Qocr.Generator.Data;
using Qocr.Tester.Helpers;

using FontFamily = System.Drawing.FontFamily;
using FontStyle = System.Drawing.FontStyle;

namespace Qocr.Tester.Windows.ViewModels
{
    public class MainWindowViewModel : NotificationObject
    {
        private ImageSource _approximatedImage;

        private ImageSource _sourceImage;

        private readonly EulerGenerator _generator = new EulerGenerator();

        private bool _genStated;

        private void TestGen()
        {
            using (FileStream fileStream = new FileStream("Gen.bin", FileMode.Open))
            {
                var container = CompressionUtils.Decompress<EulerContainer>(fileStream);
            }
        }

        private bool _isPastProcessing;

        private ImageSource _currentGenImage;

        private Font _currentFont;

        public MainWindowViewModel()
        {
            AnalyzeCommand = new DelegateCommand(Analyze);
            OpenSourceImageCommand = new DelegateCommand(OpenSourceImage);
            ImagePastCommand = new DelegateCommand(ImagePast);
            GenCommand = new DelegateCommand(GenStart, CanGen);
            TestCommand = new DelegateCommand(Test);
            //_generator.BitmapCreated += GeneratorOnBitmapCreated;
        }

        private void Analyze()
        {
            TextRecognizer recognizer = new TextRecognizer();
            var bitmap = BitmapUtils.BitmapFromSource((BitmapSource)ApproximatedImage);
            var report = recognizer.Recognize(bitmap);

            //report.RawText()
        }

        private void Test()
        {
            var debugFolder = new DirectoryInfo(@"BmpDebug");
            foreach (var file in debugFolder.GetFiles())
            {
                file.Delete();
            }

            TextRecognizer recognizer = new TextRecognizer();
            var bm = (Bitmap)Image.FromFile("MiniTest.png");
            recognizer.PrintTest = PrintTest;
            var q= recognizer.Recognize(bm);
        }

        private int debugCount = 0;
        private void PrintTest(IMonomap monomap)
        {

            monomap.ToBitmap().Save($"BmpDebug\\{debugCount}.png", ImageFormat.Png);
            debugCount ++;
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
                    var appx = new LuminosityApproximator();
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

        public DelegateCommand TestCommand { get; private set; }
        
        public DelegateCommand GenCommand { get; private set; }

        public DelegateCommand ImagePastCommand { get; private set; }

        public DelegateCommand OpenSourceImageCommand { get; private set; }

        public DelegateCommand AnalyzeCommand { get; private set; }

        private bool CanGen()
        {
            return !_genStated;
        }

        private void RecreateTestDir()
        {
            if (Directory.Exists("BmpDebug"))
            {
                Directory.Delete("BmpDebug", true);
            }

            Directory.CreateDirectory("BmpDebug");
            while (!Directory.Exists("BmpDebug")) ;
        }


        private async void GenStart()
        {
            if (File.Exists("Gen.bin"))
            {
                TestGen();
                if (MessageBox.Show("Файл Gen.bin существует, заменить ?", "Q?", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    return;
                }
            }
            
            _genStated = true;
            GenCommand.RaiseCanExecuteChanged();
            _genImageNumber = 0;

            RecreateTestDir();

            DateTime dNow = DateTime.Now;

            EulerContainer container = new EulerContainer();
            const int MinFont = 8;
            const int MaxFont = 28;

            //List<FontFamily> allowedFonts = new List<FontFamily>();
            //foreach (var fontFamily in FontFamily.Families)
            //{
            //    var tempFont = new Font(fontFamily, 15, FontStyle.Regular, GraphicsUnit.Pixel);
            //    var preview = EulerGenerator.PrintChar('Ъ', tempFont);
            //    CurrentGenImage = BitmapUtils.SourceFromBitmap(preview);

            //    if (MessageBox.Show("Используем ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //    {
            //        allowedFonts.Add(fontFamily);
            //    }
            //}

            //var fontFamilies = allowedFonts.ToArray();

            var fontFamilies = new[]
            {
                new FontFamily("Times New Roman"),
                new FontFamily("Arial"),
                new FontFamily("Courier"),
                FontFamily.GenericSansSerif,
                FontFamily.GenericSerif
            };

            var ruLang = await GenerateLanguage("RU-ru", MinFont, MaxFont, 'а', 'я', fontFamilies);
            var enLang = await GenerateLanguage("EN-en", MinFont, MaxFont, 'a', 'z', fontFamilies);
            var specialChars = new[]
            {
                '@',
                '$',
                '#',
                '&',
                '(',
                ')',
                '*',
                '/',
                '\\'
            };
            var specialCharsResult = await _generator.GenerateSpecialChars(specialChars, MinFont, MaxFont, fontFamilies);

            ruLang.FontFamilyNames = enLang.FontFamilyNames = fontFamilies.Select(font => font.Name).ToList();

            container.Languages.Add(ruLang);
            container.Languages.Add(enLang);
            container.SpecialChars = specialCharsResult;

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

        private async Task<Language> GenerateLanguage(string localization, int minFont, int maxFont, char startChr, char endChr, FontFamily[] fontFamilies)
        {
            List<char> chars = new List<char>();
            for (char c = startChr; c <= endChr; c++)
            {
                chars.Add(c);
            }
            
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
                        try
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

                            
                                CurrentGenImage = BitmapUtils.SourceFromBitmap(bmp);
                            
                            

                            _genImageNumber ++;
                            bmp.Dispose();
                            CurrentFont = args.CurrentFont;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                CurrentGenImage = IconToImageSource(SystemIcons.Error);
                            }
                            catch (Exception)
                            {
                                
                            }
                            
                        }
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