using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

using Qocr.Core.Data;
using Qocr.Core.Data.Serialization;
using Qocr.Core.Interfaces;
using Qocr.Core.Recognition;
using Qocr.Generator.Data;

namespace Qocr.Generator
{
    /// <summary>
    /// Генератор значений Эйлеровой характеристики.
    /// </summary>
    public class EulerGenerator
    {
        private const int ImageBound = 10;
        
        /// <summary>
        /// Событие об окончании создания <see cref="Bitmap"/>.
        /// </summary>
        public event EventHandler<BitmapEventArgs> BitmapCreated;

        public async Task<Language> GenerateLanguage(
            char[] sourceChars,
            int lowestMinSize,
            int largestMaxSize,
            string localizationName,
            FontFamily[] fontFamilies)
        {
            if (string.IsNullOrEmpty(localizationName))
            {
                throw new ArgumentNullException(nameof(localizationName));
            }

            Language language = new Language { LocalizationName = localizationName };
            language.MaxFontSize = largestMaxSize;
            language.MinFontSize = lowestMinSize;

            await Task.Factory.StartNew(
                () =>
                {
                    language.LowcaseCharactors.AddRange(
                        sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));
                    language.UppercaseCharactors.AddRange(
                        sourceChars.Select(@char => new Symbol { Chr = char.ToUpper(@char) }));

                    InternalGenerateEulerCollections(
                        sourceChars,
                        lowestMinSize,
                        largestMaxSize,
                        language.LowcaseCharactors,
                        language.UppercaseCharactors,
                        fontFamilies);

                    
                });

            return language;
        }

        public async Task<SpecialChars> GenerateSpecialChars(
            char[] sourceChars,
            int lowestMinSize,
            int largestMaxSize,
            FontFamily[] fontFamilies)
        {
            return await Task.Factory.StartNew(
                () =>
                {
                    SpecialChars specialChars = new SpecialChars();

                    specialChars.LowcaseCharactors.AddRange(
                        sourceChars.Select(@char => new Symbol { Chr = char.ToLower(@char) }));
                    specialChars.UppercaseCharactors.AddRange(
                        sourceChars.Select(@char => new Symbol { Chr = char.ToUpper(@char) }));

                    InternalGenerateEulerCollections(
                        sourceChars,
                        lowestMinSize,
                        largestMaxSize,
                        specialChars.LowcaseCharactors,
                        specialChars.UppercaseCharactors,
                        fontFamilies);

                    return specialChars;
                });
        }

        private static Bitmap PrintChar(char chr, Font font)
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

        private void InternalGenerateEulerCollections(
            char[] sourceChars,
            int lowestMinSize,
            int largestMaxSize,
            List<Symbol> lowcaseColleciton,
            List<Symbol> uppercaseColleciton,
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
                        var newFont = new Font(fontFamily, lowestMinSize, fontStyle);
                        InternalGenerateEulerValue(lowcaseChars, newFont, lowestMinSize, largestMaxSize, lowcaseColleciton);
                        InternalGenerateEulerValue(uppercaseChars, newFont, lowestMinSize, largestMaxSize, uppercaseColleciton);
                    }
                });
            
        }

        private void InternalGenerateEulerValue(
            char[] sourceChars,
            Font font,
            int lowestMinSize,
            int largestMaxSize,
            List<Symbol> symbols)
        {
            foreach (char chr in sourceChars)
            {
                for (int size = lowestMinSize; size < largestMaxSize + 1; size++)
                {
                    // TODO Что бы красиво выводить побуквенно идут по размерам каждой буквы, иначе тут лучше не вертикально а горизонтально по слою проходить
                    Font newFont = new Font(font.FontFamily, size, font.Style, GraphicsUnit.Pixel);
                    var bitmap = PrintChar(chr, newFont);
                    IMonomap monomap = new Monomap(bitmap);
                    var euler = EulerCharacteristicComputer.Compute2D(monomap);

                    var chr1 = chr;
                    Symbol symbol = symbols.First(s => s.Chr == chr1);
                    switch (font.Style)
                    {
                        case FontStyle.Bold:
                            symbol.CodesBold.Add(euler);
                            break;
                        case FontStyle.Italic:
                            symbol.CodesItalic.Add(euler);
                            break;
                        case FontStyle.Regular:
                            symbol.CodesNormal.Add(euler);
                            break;
                        default:
                            throw new NotSupportedException(font.Style.ToString());
                    }

                    BitmapCreated?.Invoke(this, new BitmapEventArgs((Bitmap)bitmap.Clone(), newFont, chr));
                }
            }
        }
    }
}