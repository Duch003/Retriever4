using NUnit.Framework;
using Retriever4.Validation;
using System;

namespace Retriever4.Tests
{
    [TestFixture]
    public class DoubleValidationTests
    {
        [TestCase(-13, 12, true)]
        [TestCase(0, 12, true)]
        [TestCase(7.8, 12, true)]
        [TestCase(12, 12, true)]
        [TestCase(12.01, 12, false)]
        [TestCase(14, 12, false)]
        public void CompareWearlevel_LogicTest(double real, double constant, bool expected)
        {
            var anwser = DoubleValidation.CompareWearLevel(constant, real);
            Assert.IsTrue(expected == anwser);
        }

        [TestCase("256 GB SSD", 274877906944, true)]
        [TestCase("128 GB SSD", 137438953472, true)]
        [TestCase("64 GB eMMC", 68719476736, true)]
        [TestCase("32 GB eMMC", 34359738368, true)]
        [TestCase("16 GB eMMC", 17179869184, true)]
        [TestCase("320 GB HDD", 343597383680, true)]
        [TestCase("500 GB HDD", 536870912000, true)]
        [TestCase("640 GB SSHD", 687194767360, true)]
        [TestCase("750 GB SSHD", 805306368000, true)]
        [TestCase("1 TB HDD", 1099511627776, true)]
        [TestCase("1,5 TB DHDD", 1649267441664, true)]
        [TestCase("2 TB HDD", 2199023255552, true)]
        [TestCase("0,5 TB HDD", 549755813888, true)]
        [TestCase("0,5 TB HDD", 1099511627776, false)]
        [TestCase("320 GB HDD", 274877906944, false)]
        [TestCase("16 GB eMMC", 34359738368, false)]
        public void CompareStorages_VariousData(string db, double real, bool expected)
        {
            var result = DoubleValidation.CompareStorages(db, real);
            Assert.IsTrue(result == expected);
        }

        [TestCase("", 100)]
        [TestCase(null, 100)]
        [TestCase("Test", -3)]
        [TestCase("", -6)]
        [TestCase(null, -9)]
        public void CompareStorages_DatabaseArgumentIsNullOrRealValueIsNegative_ThrowsException(string db, double real)
        {
            Assert.Throws<ArgumentException>(() => DoubleValidation.CompareStorages(db, real));
        }

    }
}
