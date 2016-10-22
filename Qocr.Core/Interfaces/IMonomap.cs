namespace Qocr.Core.Interfaces
{
    /// <summary>
    /// Монохромное изображение.
    /// </summary>
    public interface IMonomap
    {
        bool this[int x, int y] { get; }

        int Width { get; }

        int Height { get; }
    }
}
