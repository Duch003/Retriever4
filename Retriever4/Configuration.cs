using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4
{
    public class Configuration
    {
        public string Filepath = Environment.CurrentDirectory;
        public string Filename = @"\NoteBookiRef_v3.xlsm";
        public string DatabaseTableName = "MD";
        public string BiosTableName = "BIOS";
        public int? MD_ModelColumn = 0;
        public int? MD_MsnColumn = 1;
        public int? MD_OldMsnColumn = 2;
        public int? MD_CaseModelColumn = 13;
        public int? MD_PeaqModelColumn = 17;
        public int? BIOS_CaseModelColumn = 0;
        public int? BIOS_BiosVersion = 3;

        public Configuration() { }

        public bool CheckFiledsForNulls()
        {
            return Filepath == null || Filename == null || DatabaseTableName == null || BiosTableName == null || MD_ModelColumn == null ||
                MD_MsnColumn == null || MD_OldMsnColumn == null || MD_CaseModelColumn == null || MD_PeaqModelColumn == null ||
                BIOS_BiosVersion == null || BIOS_CaseModelColumn == null;
        }

    }
}
