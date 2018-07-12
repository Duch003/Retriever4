using Retriever4.Enums;

namespace Retriever4
{
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
    }
}
