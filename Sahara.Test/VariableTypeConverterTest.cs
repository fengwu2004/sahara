using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sahara.Core.Utils;
using System.Collections.Generic;
using Sahara.Core;

namespace Sahara.Test
{
    [TestClass]
    public class VariableTypeConverterTest
    {
        [TestMethod]
        public void Should_BeAbleTo_Convert_Array()
        {
            var parsedArray = (List<object>)VariableTypeConverter.Convert("{1000, \"a\"}");
            Assert.IsTrue(parsedArray.Count == 2);
            Assert.AreEqual<uint>(1000, (uint)parsedArray[0]);
            Assert.AreEqual<string>("a", parsedArray[1].ToString());

            var emptyArray = (List<object>)VariableTypeConverter.Convert("{\"\", \"\", \"\"}");
            Assert.IsTrue(emptyArray.Count == 3);
        }

        [TestMethod]
        public void Should_BeAbleTo_Convert_NestedArray()
        {
            var array = (List<object>)VariableTypeConverter.Convert("{1000, {+100,\"a\"}}");
            Assert.IsTrue(array.Count == 2);
            Assert.AreEqual<uint>(1000, (uint)array[0]);
            Assert.IsInstanceOfType(array[1], typeof(List<object>));

            var nestedArray = (List<object>)array[1];
            Assert.AreEqual<int>(100, (int)nestedArray[0]);
            Assert.AreEqual<string>("a", nestedArray[1].ToString());
        }

        [TestMethod]
        public void Should_BeAbleTo_Convert_Boolean()
        {
            var parsedBoolean = (bool)VariableTypeConverter.Convert("T");
            Assert.IsTrue(parsedBoolean);
            parsedBoolean = (bool)VariableTypeConverter.Convert("F");
            Assert.IsFalse(parsedBoolean);
        }

        [TestMethod]
        public void Should_BeAbleTo_Convert_Uint()
        {
            var parsedUint = (uint)VariableTypeConverter.Convert("1000");
            Assert.AreEqual<uint>(1000, parsedUint);
        }

        [TestMethod]
        public void Should_BeAbleTo_Convert_Int()
        {
            var parsedInt = (int)VariableTypeConverter.Convert("+1000");
            Assert.AreEqual<int>(1000, parsedInt);
            var negativeInt = (int)VariableTypeConverter.Convert("-1000");
            Assert.AreEqual<int>(-1000, negativeInt);
        }
    }
}
