using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4
{
    public enum ProgramSection : byte
    {
        New,
        OS,
        SWM,
        WearLevel,
        CPU,
        Mainboard,
        Bios,
        RAM,
        Storage,
        Shipping,
        MainboardModel
    }

    public class Container
    {
        public ProgramSection? Section = null;
        public bool? ShowSection = null;
        public bool? UseWMI = null;
        public string Header = null;
        public object WMIResult { get; set; }
        public string Query = null;
        public string Property = null;
        public string Scope = null;
        public bool? UseConstant = null;
        public object DatabaseResult { get; set; }
        public string Table = null;
        public dynamic Constant = null;
        public int? Column = null;
        public dynamic HiddenConstant { get; set; }

        public Container() { }

        //Zwraca true jeżeli występują null
        public bool CheckRequiredFields()
        {
            bool result = true;
            if (Section == null && ShowSection == null && UseWMI == null && UseConstant == null)
                return result;
            if ((bool)UseConstant)
            {
                result = result && Constant == null;
                result = result && !(Table == null);
                result = result && !(Column == null);
            }
            else
            {
                result = result && string.IsNullOrEmpty(Table);
                result = result && (Column == null);
                result = result && !(Constant == null);
            }
            if (Section == ProgramSection.New)
            {
                result = result && string.IsNullOrEmpty(Header);
            }
            else
            {
                result = result && !string.IsNullOrEmpty(Header);
            }
            if ((bool)UseWMI)
            {
                result = result && string.IsNullOrEmpty(Property);
                result = result && string.IsNullOrEmpty(Scope);
                result = result && string.IsNullOrEmpty(Table);
            }

            return result;
        }

        public bool StringsContains(string a, string b)
        {
            return b.Contains(a) || a.Contains(b);
        }

        public bool StringsEquals(string a, string b)
        {
            return a.Equals(b);
        }

        public bool NotNullOrEmpty(string a)
        {
            return !string.IsNullOrEmpty(a);
        }

        public bool NumbersEquals(double a, double b)
        {
            return a.Equals(b);
        }

        public bool NumbersGreatherThan(double a, double b)
        {
            return a > b;
        }

        public bool NumbersSmallerThan(double a, double b)
        {
            return a < b;
        }

        public bool NumbersGreatherThanOrEqual(double a, double b)
        {
            return a >= b;
        }

        public bool NumbersSmallerThanOrEqual(double a, double b)
        {
            return a <= b;
        }
    }
}
