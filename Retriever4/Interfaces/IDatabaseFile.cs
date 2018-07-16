namespace Retriever4.Interfaces
{
    public interface IDatabaseFile
    {
        bool DoesDatabaseFileExists(string filepath, string filename);
        object ReadDetailsFromDatabase(string filepath, string filename, string tableName, int row, int column);
    }
}
