using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qocr.Core.Data;
using Qocr.Core.Interfaces;

namespace Qocr.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        private const string ImagesPathTemplate = "Qocr.UnitTests.Images.{0}";

        private readonly string _taxiPng = string.Format(ImagesPathTemplate, "taxi.png");

        [TestMethod]
        public void TaxiSize()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_taxiPng))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                IMonomap monomap = new Monomap(bitmap);
                Assert.AreEqual(monomap.Height, 2);
                Assert.AreEqual(monomap.Width, 5);
            }
        }

        [TestMethod]
        public void TaxiCellValues()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_taxiPng))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                IMonomap monomap = new Monomap(bitmap);
                Assert.AreEqual(monomap[0, 0], true);
                Assert.AreEqual(monomap[1, 0], false);
                Assert.AreEqual(monomap[2, 0], true);
                Assert.AreEqual(monomap[3, 0], false);
                Assert.AreEqual(monomap[4, 0], true);

                Assert.AreEqual(monomap[0, 1], false);
                Assert.AreEqual(monomap[1, 1], true);
                Assert.AreEqual(monomap[2, 1], false);
                Assert.AreEqual(monomap[3, 1], true);
                Assert.AreEqual(monomap[4, 1], false);
            }
        }
    }
}
