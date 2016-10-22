using System.Drawing;

namespace Qocr.Core.Interfaces
{
    public interface IApproximator
    {
        IMonomap Approximate(Bitmap image);
    }
}
