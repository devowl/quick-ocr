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
using Qocr.Core.Data;
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
        public string _eulerValue;

        private readonly EulerGenerator _generator = new EulerGenerator();

        private ImageSource _approximatedImage;

        private ImageSource _sourceImage;

        private bool _genStated;

        private bool _isPastProcessing;

        private ImageSource _currentGenImage;

        private Font _currentFont;

        private int debugCount = 0;

        private int _genImageNumber = 0;

        private TextRecognizer _recognizer;

        public MainWindowViewModel()
        {
            AnalyzeCommand = new DelegateCommand(Analyze);
            OpenSourceImageCommand = new DelegateCommand(OpenSourceImage);
            ImagePastCommand = new DelegateCommand(ImagePast);
            GenCommand = new DelegateCommand(GenStart, CanGen);
            TestCommand = new DelegateCommand(Test);

            //_generator.BitmapCreated += GeneratorOnBitmapCreated;
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

        public string EulerValue
        {
            get
            {
                return _eulerValue;
            }
            set
            {
                _eulerValue = value;
                RaisePropertyChanged(() => EulerValue);
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
                if (value != null)
                {
                    EulerValue =
                        EulerCharacteristicComputer.Compute2D(
                            new Monomap(BitmapUtils.BitmapFromSource((BitmapSource)value))).ToString();
                }
            }
        }

        public DelegateCommand TestCommand { get; private set; }

        public DelegateCommand GenCommand { get; private set; }

        public DelegateCommand ImagePastCommand { get; private set; }

        public DelegateCommand OpenSourceImageCommand { get; private set; }

        public DelegateCommand AnalyzeCommand { get; private set; }

        public static System.Drawing.Bitmap CombineBitmap(params Bitmap[] bitmaps)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (Bitmap bitmap in bitmaps)
                {
                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    //g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                {
                    finalImage.Dispose();
                }

                throw ex;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

        private static ImageSource IconToImageSource(Icon icon)
        {
            var imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        private EulerContainer GetEulerContainer(string path)
        {
            using (var english = File.Open(path, FileMode.Open))
            {
                return CompressionUtils.Decompress<EulerContainer>(english);
            }
        }

        private void Analyze()
        {
            //var dic = GetEulerContainer(@"..\..\..\Qocr.Dics\RU-ru.bin");
            var dic = GetEulerContainer(@"..\..\..\Qocr.Dics\EN-en.bin");

            DateTime nowInit = DateTime.Now;

            // ИСПОЛЬЗУЙ Gen.bin
            _recognizer = _recognizer ?? new TextRecognizer(dic);

            DateTime nowRecognition = DateTime.Now;
            var bitmap = BitmapUtils.BitmapFromSource((BitmapSource)ApproximatedImage);
            var report = _recognizer.Recognize(bitmap);
            MessageBox.Show(
                $"Init time: {nowRecognition - nowInit}\n\rRecognition time: {DateTime.Now - nowRecognition}");

            RecognitionVisualizerUtils.Visualize(bitmap, report);
            ApproximatedImage = BitmapUtils.SourceFromBitmap(bitmap);
        }

        private void Test()
        {
            //var debugFolder = new DirectoryInfo(@"BmpDebug");
            //foreach (var file in debugFolder.GetFiles())
            //{
            //    file.Delete();
            //}

            //TextRecognizer recognizer = new TextRecognizer();
            //var bm = (Bitmap)Image.FromFile("MiniTest.png");

            //// recognizer.PrintTest = PrintTest;
            //var q = recognizer.Recognize(bm);
        }

        private void PrintTest(IMonomap monomap)
        {
            monomap.ToBitmap().Save($"BmpDebug\\{debugCount}.png", ImageFormat.Png);
            debugCount ++;
        }

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
            while (!Directory.Exists("BmpDebug"))
            {
                ;
            }
        }

        private async void GenStart()
        {
            if (File.Exists("Gen.bin"))
            {
                //TestGen();
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
            const int MaxFont = 25;

            var fontFamilies = ManualChoose().ToArray();

            //var fontFamilies = new[]
            //{
            //    new FontFamily("Times New Roman"),
            //    new FontFamily("Arial"),
            //    new FontFamily("Courier"),
            //    FontFamily.GenericSansSerif,
            //    FontFamily.GenericSerif
            //};

            _generator.BitmapCreated += GeneratorOnBitmapCreated;

            //var enLang = await GenerateLanguage("RU-ru", MinFont, MaxFont, 'а', 'я', fontFamilies);
            var enLang = await GenerateLanguage("EN-en", MinFont, MaxFont, 'a', 'z', fontFamilies);
            var specialChars = new[]
            {
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
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

            //ruLang.FontFamilyNames = 
            enLang.FontFamilyNames = fontFamilies.Select(font => font.Name).ToList();

            //container.Languages.Add(ruLang);
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

        private IEnumerable<FontFamily> ManualChoose()
        {
            const string LastSelectionsFileName = "LastFonts.txt";

            if (File.Exists(LastSelectionsFileName))
            {
                if (
                    MessageBox.Show(
                        $"Файл {LastSelectionsFileName} существует. Использовать?",
                        null,
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var lastFonts = File.ReadAllLines(LastSelectionsFileName);
                    return lastFonts.Select(font => FontFamily.Families.First(f => f.Name == font)).ToArray();
                }
            }

            List<FontFamily> allowedFonts = new List<FontFamily>();
            foreach (var fontFamily in FontFamily.Families)
            {
                var fontStyle = fontFamily.IsStyleAvailable(FontStyle.Regular)
                    ? FontStyle.Regular
                    : fontFamily.IsStyleAvailable(FontStyle.Italic) ? FontStyle.Italic : FontStyle.Bold;

                var tempFont = new Font(fontFamily, 10, fontStyle, GraphicsUnit.Pixel);
                var preview = new[]
                {
                    EulerGenerator.PrintChar('ъ', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('ф', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('е', tempFont).ToBitmap(),
                    
                    EulerGenerator.PrintChar('А', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('Ы', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('ф', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('у', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('ю', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('я', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('а', tempFont).ToBitmap(),
                    EulerGenerator.PrintChar('м', tempFont).ToBitmap(),
                };

                CurrentGenImage = BitmapUtils.SourceFromBitmap(CombineBitmap(preview));

                if (MessageBox.Show($"\"{fontFamily.Name}\" Используем ?", "", MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    allowedFonts.Add(fontFamily);
                }
            }

            File.WriteAllLines(LastSelectionsFileName, allowedFonts.Select(font => font.Name));
            return allowedFonts.ToArray();
        }

        private async Task<Language> GenerateLanguage(
            string localization,
            int minFont,
            int maxFont,
            char startChr,
            char endChr,
            FontFamily[] fontFamilies)
        {
            List<char> chars = new List<char>();
            for (char c = startChr; c <= endChr; c++)
            {
                chars.Add(c);
            }

            return await _generator.GenerateLanguage(chars.ToArray(), minFont, maxFont, localization, fontFamilies);
        }

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
                                while (!Directory.Exists(chrDir))
                                {
                                    ;
                                }
                            }

                            var debugFileName = $"{fontName} {fontSize} {fontStyle}.png";

                            //bmp.Save(Path.Combine(chrDir, debugFileName), ImageFormat.Png);

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