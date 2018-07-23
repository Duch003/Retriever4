using NUnit.Framework;
using Retriever4.Validation;
using System;

namespace Retriever4.Tests
{
    [TestFixture]
    public class TypeValidationTests
    {
        [TestCase(13, true)]
        [TestCase(-5, true)]
        [TestCase("lol", false)]
        [TestCase('h', false)]
        [TestCase(15342.5432, true)]
        public void IsNumericType_VariousTypes(object obj, bool expected)
        {
            Assert.IsTrue(obj.IsNumericType() == expected);
        }

        [Test]
        public void IsNumericType_NullParameter_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => TypeValidation.IsNumericType(null));
        }

        [Test]
        public void IsNullable_NullableType_ReturnsTrue()
        {
            var test = new TestClass();
            Assert.IsTrue(test.TestBool.IsNullable());
        }

        [Test]
        public void IsNullable_NullableTypeWithNullValue_ReturnsTrue()
        {
            var test = new TestClass();
            Assert.IsTrue(test.TestInt.IsNullable());
        }

        [Test]
        public void IsNullable_BasicType_ReturnsFalse()
        {
            var test = new TestClass();
            Assert.IsFalse(test.TestDouble.IsNullable());
        }

        [Test]
        public void IsNullable_BasicTypeWithNullValue_ReturnTrue()
        {
            var test = new TestClass();
            Assert.IsTrue(test.TestString.IsNullable());
        }
    }

    public class TestClass
    {
        public bool? TestBool = new bool?(false);
        public int? TestInt = null;
        public string TestString = null;
        public double TestDouble = 9;
    }
}
