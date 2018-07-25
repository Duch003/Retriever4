﻿namespace Retriever4.Interfaces
{
    public interface IDatabaseManager
    {
        object ReadDetailsFromDatabase(string tableName, int row, int column);
        bool DoesDatabaseFileExists { get; }
    }
}
