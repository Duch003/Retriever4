namespace Retriever4
{
    public class Location
    {
        public string Model;
        public string PeaqModel;
        public int BiosRow;
        public int DBRow;

        public Location(string model, string peaqModel, int dbRow, int biosRow)
        {
            Model = model;
            PeaqModel = peaqModel;
            DBRow = dbRow;
            BiosRow = biosRow;
        }

        public Location() { }
    }
}
