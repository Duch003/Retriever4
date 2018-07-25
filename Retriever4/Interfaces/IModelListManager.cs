using System.Collections.Generic;
namespace Retriever4.Interfaces
{
    public interface IModelListManager
    {
        bool DoestModelListFileExists { get; }
        bool SerializeModelList();
        List<Location> DeserializeModelList();
    }
}
