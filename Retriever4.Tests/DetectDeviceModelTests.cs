using NUnit.Framework;
using Retriever4.Validation;
using System;
using System.Collections.Generic;
using Retriever4.Classes;

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
            var model = DetectDeviceModel.DetectModel(raw);
            Assert.IsTrue(model != null && model == expected);
        }

        [TestCase("99850", "99850", 1)]
        [TestCase("43215", null, -1)]
        [TestCase("UnfitString", null, 0)]
        public void DetectDeviceModel_FindModelLogic_ReturnsTrue(string raw, string expectedModel, int expectedState)
        {
            Location model = null;
            var locations = new List<Location>
            {
                new Location("99850", "", 1, 3),
                new Location("12345", "", 2, 7),
                new Location("60150", "", 8, 3),
                new Location("60650", "", 3, 6),
                new Location("60011", "CNB C1008-I01N", 4, 0)
            };

            
            var state = DetectDeviceModel.FindModel(raw, locations, out model);
            Assert.IsTrue(state == expectedState);
        }

        [Test]
        public void DetectDeviceModel_FindModelNullRaw_ThrowsException()
        {
            Location model = null;
            var locations = new List<Location>
            {
                new Location("99850", "", 1, 3),
                new Location("12345", "", 2, 7),
                new Location("60150", "", 8, 3),
                new Location("60650", "", 3, 6),
                new Location("60011", "CNB C1008-I01N", 4, 0)
            };

            Assert.Throws<ArgumentException>(() => DetectDeviceModel.FindModel(null, locations, out model));
        }

        [Test]
        public void DetectDeviceModel_FindModelNullList_ThrowsException()
        {
            Location model = null;

            Assert.Throws<ArgumentException>(() => DetectDeviceModel.FindModel("60011", null, out model));
        }
    }
}
