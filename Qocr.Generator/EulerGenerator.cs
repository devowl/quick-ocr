using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

using Qocr.Core.Data;
using Qocr.Core.Data.Serialization;
using Qocr.Core.Interfaces;
using Qocr.Core.Recognition;
using Qocr.Core.Utils;
using Qocr.Generator.Data;

namespace Qocr.Generator
{
    /// <summary>
    /// Генератор значений Эйлеровой характеристики.
    /// </summary>
    public class EulerGenerator
    {
        private const int ImageBound = 10;

        private readonly object _syncObject = new object();

        /// <summary>
        /// Событие об окончании создания <see cref="Bitmap"/>.
        /// </summary>
        public event EventHandler<BitmapEventArgs> BitmapCreated;

        public static Bitmap PrintChar(char chr, Font font)
        {
            Bitmap bitmap = new Bitmap((int)font.Size + ImageBound * 2, (int)font.Size + ImageBound * 2);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                graphics.DrawString(chr.ToString(), font, Brushes.Black, ImageBound, ImageBound);
                graphics.Flush();
            }

            return bitmap;
        }

        public async Task<Language> GenerateLanguage(
            char[] sourceChars,
            int minSize,
            int maxSize,
            string localizationName,
            FontFamily[] fontFamilies)
        {
            if (string.IsNullOrEmpty(localizationName))
            {
                throw new ArgumentNullException(nameof(localizationName));
            }

            Language language = new Language { LocalizationName = localizationName };

            await Task.Factory.StartNew(
                () =>
                {
                    language.Chars.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));
                    language.Chars.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToUpper(@char) }));

                    InternalGenerateEulerCollections(
                        sourceChars,
                        minSize,
                        maxSize,
                        language.Chars,
                        fontFamilies);
                });

            return language;
        }

        public async Task<List<Symbol>> GenerateSpecialChars(
            char[] sourceChars,
            int minSize,
            int maxSize,
            FontFamily[] fontFamilies)
        {
            return await Task.Factory.StartNew(
                () =>
                {
                    var result = new List<Symbol>();
                    result.AddRange(sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));

                    InternalGenerateEulerCollections(sourceChars, minSize, maxSize, result, fontFamilies);

                    return result;
                });
        }

        private void InternalGenerateEulerCollections(
            char[] sourceChars,
            int minSize,
            int maxSize,
            List<Symbol> charColleciton,
            FontFamily[] fontFamilies)
        {
            var lowcaseChars = sourceChars.Select(char.ToLower).ToArray();
            var uppercaseChars = sourceChars.Select(char.ToUpper).ToArray();

            var styles = new[]
            {
                FontStyle.Bold,
                FontStyle.Regular,
                FontStyle.Italic,
            };

            Parallel.ForEach(
                fontFamilies,
                fontFamily =>
                {
                    foreach (var fontStyle in styles)
                    {
                        var newFont = new Font(fontFamily, minSize, fontStyle);
                        InternalGenerateEulerValue(lowcaseChars, newFont, minSize, maxSize, charColleciton);
                        InternalGenerateEulerValue(uppercaseChars, newFont, minSize, maxSize, charColleciton);
                    }
                });
        }

        private object _saveLock = new object();

        private void InternalGenerateEulerValue(
            char[] sourceChars,
            Font font,
            int minSize,
            int maxSize,
            List<Symbol> symbols)
        {
            foreach (char chr in sourceChars)
            {
                for (int size = minSize; size < maxSize + 1; size++)
                {
                    // TODO Что бы красиво выводить побуквенно идут по размерам каждой буквы, иначе тут лучше не вертикально а горизонтально по слою проходить
                    using (var newFont = new Font(font.FontFamily, size, font.Style, GraphicsUnit.Pixel))
                    {
                        using (var bitmap = PrintChar(chr, newFont))
                        {
                            IMonomap monomap = new Monomap(bitmap);
                            int height = GetFontHeight(monomap);

                            if (height < minSize)
                            {
                                continue;
                            }

                            var euler = EulerCharacteristicComputer.Compute2D(monomap);

                            var chr1 = chr;
                            Symbol symbol = symbols.First(s => s.Chr == chr1);
                            SymbolCode symbolCode = new SymbolCode(height, euler);

                            lock (_syncObject)
                            {
                                symbol.Codes.Add(symbolCode);
                            }

                            BitmapCreated?.Invoke(this, new BitmapEventArgs((Bitmap)bitmap.Clone(), newFont, chr));
                        }
                    }
                }
            }
        }

        private int GetFontHeight(IMonomap monomap)
        {
            int topY = -1, bottomY = -1;

            for (int y = 0; y < monomap.Height; y++)
            {
                for (int x = 0; x < monomap.Width; x++) 
                {
                    if (monomap[x, y])
                    {
                        topY = y;
                    }
                }
            }

            for (int y = monomap.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < monomap.Width; x++) 
                {
                    if (monomap[x, y])
                    {
                        bottomY = y;
                    }
                }
            }
            
            return topY - bottomY + 1;
        }
    }
}