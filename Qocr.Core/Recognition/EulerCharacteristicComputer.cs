using System.Collections.Generic;
using System.Linq;

using Qocr.Core.Data.Map2D;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Recognition
{
    /// <summary>
    /// Класс для вычисления Эйлеровой характеристики изображения.
    /// </summary>
    internal class EulerCharacteristicComputer
    {
        /// <summary>
        /// Высчитать эйлеровоую характеристику для 2D imageSource.
        /// </summary>
        /// <param name="imageSource">Ссылка на изображение.</param>
        /// <param name="eulerSquares">Набор фрагментов.</param>
        /// <returns>Эйлеровая характеристика.</returns>
        public EulerMonomap2D Compute2D(IMonomap imageSource, IEnumerable<IEulerSquare> eulerSquares)
        {
            var eulerSquaresList = eulerSquares.ToList();
            Dictionary<string, int> eulerValue = eulerSquaresList.ToDictionary(item => item.SquareIdent, item => 0);
            var fragment2DSize = 2;
            for (int y = 0; y < imageSource.Height - fragment2DSize - 1; y++)
            {
                for (int x = 0; x < imageSource.Width - fragment2DSize - 1; x++)
                {
                    for (int i = 0; i < eulerSquaresList.Count; i++)
                    {
                        var eulerSquare = eulerSquaresList[i];
                        if (eulerSquare.IsSquareDetected(x, y, imageSource))
                        {
                            eulerValue[eulerSquare.SquareIdent]++;
                            break;
                        }
                    }
                }
            }

            return new EulerMonomap2D(eulerValue);
        }
    }
}