using System.Collections.Concurrent;

namespace NimbleRegistry;

public class RegistryAlreadyExists : Exception
{
    public RegistryAlreadyExists(string? message) : base(message)
    {
    }
}

public class RegistryStore<T> where T : IRegistry
{
    private readonly ConcurrentDictionary<string, T> registries;
    private readonly IRegistryFactory<T> registryFactory;

    public RegistryStore(IRegistryFactory<T> registryFactory)
    {
        this.registryFactory = registryFactory;
        registries = new ConcurrentDictionary<string, T>();
    }


    public IEnumerable<string> GetIds()
    {
        return registries.Keys;
    }

    public bool HasId(string id)
    {
        return registries.ContainsKey(id);
    }

    public T Create(string id)
    {
        if (HasId(id)) throw new RegistryAlreadyExists($"Registry with id {id} already exists.");

        return registries[id] = registryFactory.Create(id);
    }

    public T? Get(string id)
    {
        return registries.GetValueOrDefault(id);
    }

    public T CreativeGet(string id)
    {
        return HasId(id) ? Get(id)! : Create(id);
    }

    public bool Delete(string id)
    {
        
        if (!registries.TryGetValue(id, out var reg))
            return false;

        // TODO: we have parallelism breach here that may happens in extreme traffics. Mutex, etc. can be added later.
        
        registries.Remove(id, out _);
        reg.Drop();
        return true;
    }
}