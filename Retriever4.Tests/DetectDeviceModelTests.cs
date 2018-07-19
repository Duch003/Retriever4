using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using NUnit.Framework;
using Retriever4.Utilities;
using Retriever4.Validation;

namespace Retriever4.Tests
{
    [TestFixture]
    public class DetectDeviceModelTests
    {
        [TestCase("99850", "99850")]
        [TestCase("C1010-I01B3", "C1010-I01B3")]
        [TestCase("S1415-I2N1S", "S1415-I2N1S")]
        [TestCase("C2015-I5N1", "C2015-I5N1")]
        [TestCase("S1115-IMNL", "S1115-IMNL")]
        [TestCase("P1115-I5CH", "P1115-I5CH")]
        [TestCase("C1008-I01N", "C1008-I01N")]
        [TestCase("S1115-IM2N", "S1115-IM2N")]
        [TestCase("S1415-I1C", "S1415-I1C")]
        [TestCase("S1414-I1BI S", "S1414-I1BI S")]
        [TestCase("PNB S1414-I1BI S", "S1414-I1BI S")]
        [TestCase("CNB MEDION S1415-I1C", "S1415-I1C")]
        [TestCase("PEAQC2015-I5N1", "C2015-I5N1")]
        [TestCase("S1415-I1C PEAQ", "S1415-I1C")]
        public void DetectDeviceModel_PatternsDetectionTests(string raw, string expected)
        {
            string model = DetectDeviceModel.DetectModel(raw);
            Assert.IsTrue(model != null && model == expected);
        }
    }
}
