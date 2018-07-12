using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4
{
    public class Location
    {
        public string Model;
        public string MSN;
        public string PeaqModel;
        public string OldMSN;
        public int BiosRow;
        public int DBRow;

        public Location(string model, string msn, string peaqModel, int dbRow, int biosRow)
        {
            Model = model;
            MSN = msn;
            PeaqModel = peaqModel;
            DBRow = dbRow;
            BiosRow = biosRow;
        }

        public Location() { }
    }
}
