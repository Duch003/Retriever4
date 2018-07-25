using NUnit.Framework;
using Retriever4.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Retriever4.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        Program.
    }

    public class MockDatabaseManager : IDatabaseManager
    {
        public bool DoesDatabaseFileExists_RETURNS;
        public object ReadDettailsFromDatabase_RETURNS;

        public bool DoesDatabaseFileExists => DoesDatabaseFileExists_RETURNS;
        public object ReadDetailsFromDatabase(string tableName, int row, int column) => ReadDettailsFromDatabase_RETURNS;
    }

    public class MockConfigFileManager : IConfigFileManager
    {
        public bool DoesConfigFileExists_RETURNS;
        public Configuration ReadConfiguration_RETURNS;
        public bool WriteConfiguration_RETURNS;

        public bool DoesConfigFileExists => DoesConfigFileExists_RETURNS;
        public Configuration ReadConfiguration() => ReadConfiguration_RETURNS;
        public bool WriteConfiguration() => WriteConfiguration_RETURNS;
    }

    public class MockModelListManager : IModelListManager
    {
        public bool DoestModelListFileExists_RETURNS;
        public List<Location> DeserializeModelList_RETURNS;
        public bool SerializeModelList_RETURNS;

        public bool DoestModelListFileExists => DoestModelListFileExists_RETURNS;
        public List<Location> DeserializeModelList() => DeserializeModelList_RETURNS;
        public bool SerializeModelList() => SerializeModelList_RETURNS;
    }

}
