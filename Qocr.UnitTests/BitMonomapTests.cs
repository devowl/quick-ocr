using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qocr.Core.Data;
using Qocr.Core.Interfaces;

namespace Qocr.UnitTests
{
    [TestClass]
    public class BitMonomapTests
    {
        private readonly bool[,] _bitImage = new bool[3, 2]
        {
            // { false, true, false },
            // { true, false, false }

            { false, true },
            { true, false },
            { false, true }
        };

        [TestMethod]
        public void EqualMatrix()
        {
            IMonomap monomap = new BitMonomap(_bitImage);

            Assert.AreEqual(monomap[0, 0], false);
            Assert.AreEqual(monomap[1, 0], true);
            Assert.AreEqual(monomap[2, 0], false);

            Assert.AreEqual(monomap[0, 1], true);
            Assert.AreEqual(monomap[1, 1], false);
            Assert.AreEqual(monomap[2, 1], true);
        }

        [TestMethod]
        public void MatrixSize()
        {
            IMonomap monomap = new BitMonomap(_bitImage);
            Assert.AreEqual(monomap.Width, 3);
            Assert.AreEqual(monomap.Height, 2);
        }
    }
}
