using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Qocr.Core.Recognition.Data;

namespace Qocr.Core.Utils
{
    /// <summary>
    /// Визуализация отчёта после распознания.
    /// </summary>
    public static class RecognitionVisualizerUtils
    {
        private static readonly Pen RectanglePen = new Pen(Color.Coral, 1);

        /// <summary>
        /// Визуализировать результат распознания на <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        public static void Visualize(Bitmap bitmap, QReport report)
        {
            var thickness = RectanglePen.Width;
            foreach (var symbol in report.Symbols)
            {
                if (symbol.State == QState.Unknown)
                {
                    continue;
                }
                
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawRectangle(RectanglePen, symbol.StartPoint.X - thickness, symbol.StartPoint.Y - thickness, symbol.Width + thickness, symbol.Height + thickness);   
                }
            }
        }
    }
}
