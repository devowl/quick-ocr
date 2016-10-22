using System.Drawing;
using Qocr.Core.Interfaces;

namespace Qocr.Tester.Helpers
{
    public static class MonomapExtentions
    {
        public static Bitmap ToBitmap(this IMonomap monomap)
        {
            var bitmap = new Bitmap(monomap.Width, monomap.Height);
            for (var y = 0; y < monomap.Height; y ++)
            {
                for (var x = 0; x < monomap.Width; x++)
                {
                    bitmap.SetPixel(x, y, monomap[x, y] ? Color.Black : Color.White);
                }
            }

            return bitmap;
        }
    }
}