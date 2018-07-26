using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4.Interfaces
{
    public interface ISHA1FileManager
    {
        bool DoesHashFileExists { get; }
        string ComputeSHA1();
        string ReadHash();
        bool WriteHash(string hash);
    }
}
