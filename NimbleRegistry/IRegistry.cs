namespace NimbleRegistry;

public interface IRegistry
{
    string Register<T>(string id, T payload) where T : IRegistryPayload;
    IEnumerable<T> Get<T>() where T : IRegistryPayload;
    IEnumerable<T> Get<T>(IEnumerable<string> ids) where T : IRegistryPayload;
    object? Get(string id);
    bool IsIdExists(string id);
    T? Get<T>(string id);
    int Size();
    DateTime GetCreationDateTime();
    DateTime GetLastSaveDateTime();
    bool Remove(string id);

    IEnumerable<object?> Get(IEnumerable<string> ids);
    void Clear();
    void Save();
}