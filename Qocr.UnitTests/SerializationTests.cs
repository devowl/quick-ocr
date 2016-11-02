using System;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qocr.Core.Data.Serialization;

namespace Qocr.UnitTests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializationValues1()
        {
            Symbol symbol = new Symbol
            {
                StringsCodesBold = "1,2,3,4,5,6,7,8,9;1,2,3,4,5,6,7,8,9"
            };

            symbol.OnDeserializedMethod(new StreamingContext());

            Assert.AreEqual(symbol.CodesBold.Count, 1);
        }

        [TestMethod]
        public void SerializationValues2()
        {
            Symbol symbol = new Symbol
            {
                StringsCodesBold = "1,2,3,4,5,6,7,8,9;1,2,3,4,5,6,7,8,9;"
            };

            symbol.OnDeserializedMethod(new StreamingContext());
            Assert.AreEqual(symbol.CodesBold.Count, 1);
        }
        
        [TestMethod]
        public void SerializationValues3()
        {
            Symbol symbol = new Symbol
            {
                StringsCodesBold = ";"
            };

            symbol.OnDeserializedMethod(new StreamingContext());
            Assert.AreEqual(symbol.CodesBold.Count, 0);
        }

        [TestMethod]
        public void SerializationValues4()
        {
            Symbol symbol = new Symbol
            {
                StringsCodesBold = ""
            };

            symbol.OnDeserializedMethod(new StreamingContext());
            Assert.AreEqual(symbol.CodesBold.Count, 0);
        }
        
        [TestMethod]
        public void SerializationValues5()
        {
            Symbol symbol = new Symbol
            {
                StringsCodesBold = "1,2,3,4,5,6,7,8,9;1,2,3,4,5,6,7,8,9,10;"
            };

            symbol.OnDeserializedMethod(new StreamingContext());
            Assert.AreEqual(symbol.CodesBold.Count, 2);
        }
    }
}
