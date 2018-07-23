using NUnit.Framework;
using Retriever4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Assert.IsTrue(TypeValidation.IsNumericType(obj) == expected);
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
            Assert.IsTrue(TypeValidation.IsNullable(test.TestBool) && TypeValidation.IsNullable(test.TestLong));
        }

        [Test]
        public void IsNullable_NullableTypeWithNullValue_ThrowsException()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentNullException>(() => TypeValidation.IsNullable(test.TestInt));
        }

        [Test]
        public void IsNullable_BasicType_ReturnsFalse()
        {
            var test = new TestClass();
            Assert.IsFalse(TypeValidation.IsNullable(test.TestDouble));
        }

        [Test]
        public void IsNullable_BasicTypeWithNullValue_ThrowsException()
        {
            var test = new TestClass();
            Assert.Throws<ArgumentNullException>(() => TypeValidation.IsNullable(test.TestString));
        }
    }

    public class TestClass
    {
        public bool? TestBool = new bool?(false);
        public int? TestInt = null;
        public string TestString = null;
        public double TestDouble = 9;
        public long? TestLong = new long?(4);

        public TestClass() { }
    }
}
