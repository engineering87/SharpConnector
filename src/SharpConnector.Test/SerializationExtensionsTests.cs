using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpConnector.Utilities;

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
        }

        private class SerializableTestClass
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}