using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Utilities;
using System.Collections.Generic;

namespace SharpConnector.Tests
{
    [TestClass]
    public class SerializationExtensionsTests
    {
        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForSerializableObject()
        {
            // Arrange
            var testObject = new SerializableTestClass { Name = "Test", Value = 42 };

            // Act
            var result = testObject.IsSerializable();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForNullObject()
        {
            // Arrange
            object testObject = null;

            // Act
            var result = testObject.IsSerializable();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForPrimitiveTypes()
        {
            // Act & Assert
            Assert.IsTrue(42.IsSerializable());
            Assert.IsTrue("test".IsSerializable());
            Assert.IsTrue(true.IsSerializable());
            Assert.IsTrue(3.14.IsSerializable());
            Assert.IsTrue('c'.IsSerializable());
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForCollection()
        {
            var list = new List<string> { "a", "b", "c" };
            Assert.IsTrue(list.IsSerializable());
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForDictionary()
        {
            var dict = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };
            Assert.IsTrue(dict.IsSerializable());
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForNestedObject()
        {
            var nested = new NestedTestClass
            {
                Id = 1,
                Inner = new SerializableTestClass { Name = "inner", Value = 99 }
            };
            Assert.IsTrue(nested.IsSerializable());
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForEmptyString()
        {
            Assert.IsTrue("".IsSerializable());
        }

        [TestMethod]
        public void IsSerializable_ReturnsTrue_ForArray()
        {
            var arr = new int[] { 1, 2, 3 };
            Assert.IsTrue(arr.IsSerializable());
        }

        private class SerializableTestClass
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private class NestedTestClass
        {
            public int Id { get; set; }
            public SerializableTestClass Inner { get; set; }
        }
    }
}