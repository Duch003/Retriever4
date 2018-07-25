namespace Retriever4.Interfaces
{
    public interface IConfigFileManager
    {
        bool DoesConfigFileExists { get; }
        Configuration ReadConfiguration();
        bool WriteConfiguration();
    }
}
