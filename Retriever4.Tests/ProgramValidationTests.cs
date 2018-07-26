using NUnit.Framework;
using Retriever4.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Retriever4.Tests.Factories;
using Retriever4.Validation;

namespace Retriever4.Tests
{
    [TestFixture]
    public class ProgramValidationTests
    {
        private Configuration Config;
        private IDrawingAtConsole mockEngine;
        private List<Location> modelList;
        private ConsoleColor pass = ConsoleColor.Green;
        private ConsoleColor fail = ConsoleColor.Red;
        private ConsoleColor warning = ConsoleColor.Yellow;

        [TearDown]
        public void Clean()
        {
            Config = null;
            modelList = null;
            mockEngine = null;
        }

        [Test]
        public void ProgramInitialization_AllValidParameters_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();

            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = new List<Location>
                {
                    new Location("99850", "", 1, 3),
                    new Location("12345", "", 2, 7),
                    new Location("60150", "", 8, 3),
                    new Location("60650", "", 3, 6),
                    new Location("60011", "CNB C1008-I01N", 4, 0),
                },
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };
            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);

            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList != null &&
                modelList.Count == 5;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }
        
        [Test]
        public void ProgramInitialization_ConfigFileDoesntExists_ReturnsFalse()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = false,
                ReadConfiguration_RETURNS = null,
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = null,
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };

            var shouldBeFalse = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config == null &&
                mockEngine != null &&
                modelList == null;
            Assert.IsTrue(!shouldBeFalse && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_ConfigFileInvalid_ReturnsFalse()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationWithNegativeNumbers(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = null,
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };

            var shouldBeFalse = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList == null;
            Assert.IsTrue(!shouldBeFalse && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_ConfigFileEmpty_ReturnsFalse()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyNull(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = null,
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };

            var shouldBeFalse = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1,ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config!= null &&
                mockEngine != null &&
                modelList == null;
            Assert.IsTrue(!shouldBeFalse && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_DatabaseFileDoesntExsists_ReturnsFalse()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = false,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = false,
                DeserializeModelList_RETURNS = null,
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };

            var shouldBeFalse = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1,ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList == null;
            Assert.IsTrue(!shouldBeFalse && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_SHA1FileDoesntExists_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = null,
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = false,
                ReadHash_RETURNS = "12345",
                WriteHash_RETURNS = true
            };

            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList == null;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_SHA1FileEmpty_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = new List<Location>
                {
                    new Location("99850", "", 1, 3),
                    new Location("12345", "", 2, 7),
                    new Location("60150", "", 8, 3),
                    new Location("60650", "", 3, 6),
                    new Location("60011", "CNB C1008-I01N", 4, 0),
                },
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = null,
                WriteHash_RETURNS = true
            };

            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList != null &&
                modelList.Count == 5;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_HashesAreDifferent_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = new List<Location>
                {
                    new Location("99850", "", 1, 3),
                    new Location("12345", "", 2, 7),
                    new Location("60150", "", 8, 3),
                    new Location("60650", "", 3, 6),
                    new Location("60011", "CNB C1008-I01N", 4, 0),
                },
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = "54321",
                WriteHash_RETURNS = true
            };

            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList != null &&
                modelList.Count == 5;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_ModelFileDoesntExists_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = false,
                DeserializeModelList_RETURNS = new List<Location>
                {
                    new Location("99850", "", 1, 3),
                    new Location("12345", "", 2, 7),
                    new Location("60150", "", 8, 3),
                    new Location("60650", "", 3, 6),
                    new Location("60011", "CNB C1008-I01N", 4, 0),
                },
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = null,
                WriteHash_RETURNS = true
            };

            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList != null &&
                modelList.Count == 5;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }

        [Test]
        public void ProgramInitialization_ModelFileEmpty_ReturnsTrue()
        {
            mockEngine = new MockDrawingAtConsole();
            var mockDB = new MockDatabaseManager
            {
                DoesDatabaseFileExists_RETURNS = true,
                ReadDettailsFromDatabase_RETURNS = null
            };
            var mockConfig = new MockConfigFileManager
            {
                DoesConfigFileExists_RETURNS = true,
                ReadConfiguration_RETURNS = ConfigurationFactory.ConfigurationFullyFilled(),
                WriteConfiguration_RETURNS = true
            };
            var mockList = new MockModelListManager
            {
                DoestModelListFileExists_RETURNS = true,
                DeserializeModelList_RETURNS = new List<Location>
                {
                    new Location("99850", "", 1, 3),
                    new Location("12345", "", 2, 7),
                    new Location("60150", "", 8, 3),
                    new Location("60650", "", 3, 6),
                    new Location("60011", "CNB C1008-I01N", 4, 0),
                },
                SerializeModelList_RETURNS = true
            };

            var mockSHA1 = new MockSHA1Manager
            {
                ComputeSHA1_RETURNS = "12345",
                DoesHashFileExists_RETURNS = true,
                ReadHash_RETURNS = null,
                WriteHash_RETURNS = true
            };

            var shouldBeTrue = ProgramValidation.Initialization(mockDB, mockConfig, mockList, mockSHA1, ref mockEngine, ref Config,
                ref modelList, pass, fail, warning);
            var initializationCheck =
                Config != null &&
                mockEngine != null &&
                modelList != null &&
                modelList.Count == 5;
            Assert.IsTrue(shouldBeTrue && initializationCheck);
        }
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

    public class MockSHA1Manager : ISHA1FileManager
    {
        public bool DoesHashFileExists_RETURNS;
        public string ComputeSHA1_RETURNS;
        public string ReadHash_RETURNS;
        public bool WriteHash_RETURNS;

        public bool DoesHashFileExists => DoesHashFileExists_RETURNS;
        public string ComputeSHA1() => ComputeSHA1_RETURNS;
        public string ReadHash() => ReadHash_RETURNS;
        public bool WriteHash(string hash) => WriteHash_RETURNS;
    }

    public class MockDrawingAtConsole : IDrawingAtConsole
    {
        public int X => 0;

        public int Y => 0;

        public int MaxX => 0;

        public int MaxY => 0;

        public void ClearRowSelection(int Y)
        {
            
        }

        public void CursorX(int Xposition)
        {
            
        }

        public void CursorY(int Yposition)
        {
            
        }

        public int PrintHorizontalLine(int startY)
        {
            return 0;
        }

        public int PrintInitializationBar(int startY, string bar)
        {
            return 0;
        }

        public int PrintInitializationComment(int Yposition, string comment, ConsoleColor color)
        {
            return 0;
        }

        public int PrintInitializationDescription(int Yposition, string title)
        {
            return 0;
        }

        public int PrintInitializationStatus(int Yposition, string status, ConsoleColor color)
        {
            return 0;
        }

        public int PrintMainHeaders(int startY)
        {
            return 0;
        }

        public void PrintModelTable(int startY, List<Location> locations)
        {

        }

        public void PrintRowSelection(int Y)
        {

        }

        public int PrintSection(int startY, string[] description, string[] leftColumnWriting, string[] rightColumnWriting, ConsoleColor color)
        {
            return 0;
        }

        public void RestoreCursorX()
        {
            
        }

        public void RestoreCursorY()
        {
            
        }

        public void Wait()
        {
        }
    }
}
