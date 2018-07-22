using NUnit.Framework;
using Retriever4.Validation;
using System;

namespace Retriever4.Tests
{
    [TestFixture]
    public class StringValidationTests
    {
        [TestCase("i5-7200U",               "GeniueIntel Intel<R> Core<TM> i5-7200U CPU @ 2.50GHz")]
        [TestCase("Pentium 4415U",          "GeniueIntel Intel<R> Pentium<TM> CPU 4415U @ 2.30GHz")]
        [TestCase("i3-7100U",               "GeniueIntel Intel<R> Core<TM> i3-7100U CPU @ 2.40GHz")]
        [TestCase("i7-8550U",               "GeniueIntel Intel<R> Core<TM> i7-8550U CPU @ 1.80GHz")]
        [TestCase("Atom x5-Z8350",          "GeniueIntel Intel<R> Atom<TM> x5-Z8350 CPU @ 1.44GHz")]
        [TestCase("Celeron N3060",          "GeniueIntel Intel<R> Celeron<TM> N3060 CPU @ 1.60GHz")]
        [TestCase("Pentium N3700",          "GeniueIntel Intel<R> Pentium<TM> N3700 CPU @ 1.60GHz")]
        [TestCase("i3-6006U",               "GeniueIntel Intel<R> Core<TM> i3-6006U CPU @ 2.00GHz")]
        [TestCase("Pentium 4405U",          "GeniueIntel Intel<R> Pentium<TM> CPU 4405U @ 2.10GHz")]
        [TestCase("Atom Z3735F",            "GeniueIntel Intel<R> Atom<TM> CPU Z3735F @ 1.33GHz")]
        [TestCase("i7-6700HQ",              "GeniueIntel Intel<R> Core<TM> i7-6700HQ CPU @ 2.60GHz")]
        [TestCase("Celeron N3150",          "GeniueIntel Intel<R> Celeron<TM> CPU N3150 @ 1.60GHz")]
        [TestCase("Pentium N4200",          "GeniueIntel Intel<R> Pentium<TM> CPU N4200 @ 1.10GHz")]
        [TestCase("Intel Celeron N2808",    "GeniueIntel Intel<R> Celeron<TM> CPU N2808 @ 1.58GHz")]
        public void CompareCPUs_VariousData_ReturnsTrue(string db, string real)
        {
            var shouldBeTrue = StringValidation.CompareCpu(db, real);
            Assert.IsTrue(shouldBeTrue);
        }

        [TestCase("Test", "")]
        [TestCase("Test", null)]
        [TestCase("", "Test")]
        [TestCase(null, "Test")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void CompareCPUs_ArgumentIsNull_ThrowsException(string db, string real)
        {
            Assert.Throws<ArgumentException>(() => StringValidation.CompareCpu(db, real));
        }

        [TestCase("E15KR", "E15KR")]
        [TestCase("E15K", "E15KUN")]
        [TestCase("E15K", "E15K")]
        [TestCase("D17K", "D17KUN")]
        [TestCase("D17KxR", "D17KR")]
        [TestCase("Nt16H", "NT16H")]
        [TestCase("E15K", "E15KGN")]
        [TestCase("E15S", "E15SIN")]
        [TestCase("Wingman", "3165 3ND", Ignore = "Potrzebna porpawka w bazie danych")] //Zamienić
        [TestCase("Polo 2", "MA50LU", Ignore = "Potrzebna porpawka w bazie danych")] //Zamienić
        [TestCase("D17KxR", "D17KGR")]
        [TestCase("WMBT1031", "CNB PEAQ P1211T MD60011", Ignore = "Potrzebna porpawka w bazie danych")]
        [TestCase("P670RE1M", "P670RE1M")]
        [TestCase("P670RE1-M", "P670RE1M")]
        [TestCase("NSBW1x02", "NSBW1402")]
        [TestCase("WMBT1031", "WMBT8931", Ignore = "Potrzebna porpawka w bazie danych")] //Niesprecyzowane w bazie, trzeba dodać po średniku wartość WMBT8931, 60011
        [TestCase("NT13A", "NT13A")]
        [TestCase("Skoda", "E1232T", Ignore = "Potrzebna porpawka w bazie danych")] //99466
        [TestCase("D17KxR", "D17KRR")]
        [TestCase("D17S", "D17SFN")]
        [TestCase("NSSL1502", "NSSL1502")]
        [TestCase("NSBW1x02", "NSBW1502")]
        public void CompareMainboardModel_VariousData_ReturnsTrue(string db, string real)
        {
            var shouldBeTrue = StringValidation.CompareMainboardModel(db, real);
            Assert.IsTrue(shouldBeTrue);
        }

        [TestCase("Test", "")]
        [TestCase("Test", null)]
        [TestCase("", "Test")]
        [TestCase(null, "Test")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void CompareMainboardModel_ArgumentIsNull_ThrowsException(string db, string real)
        {
            Assert.Throws<ArgumentException>(() => StringValidation.CompareMainboardModel(db, real));
        }

        [TestCase("Test", "")]
        [TestCase("Test", null)]
        [TestCase("", "Test")]
        [TestCase(null, "Test")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void CompareStrings_ArgumentIsNull_ThrowsException(string left, string right)
        {
            Assert.Throws<ArgumentException>(() => StringValidation.CompareStrings(left, right));
        }
    }

    
}
