using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;

namespace NimbleRegistry;

public class XmlRegistry : IRegistry
{
    private readonly string filePath;
    private readonly ConcurrentDictionary<string, RegistryItem> kvStore;
    private readonly Mutex mutex = new();
    public HashSet<Type> AdditionalTypes;
    
    public XmlRegistry(string filePath)
    {
        this.filePath = filePath;
        kvStore = new ConcurrentDictionary<string, RegistryItem>();

        AdditionalTypes = new HashSet<Type>();
        
        if (File.Exists(filePath))
            Read(filePath);
        else
            Save();
    }


    public string Register<T>(T payload) where T : IRegistryPayload
    {
        Register(new RegistryItem(payload.Id, payload));
        return payload.Id;
    }

    public IEnumerable<T> Get<T>() where T : IRegistryPayload
    {
        return kvStore.Values
            .Where(
                r =>
                    r.Payload.GetType().IsAssignableFrom(typeof(T)) ||
                    r.Payload is T
            )
            .Select(r => (T) r.Payload);
    }

    public IEnumerable<T> Get<T>(IEnumerable<string> ids) where T : IRegistryPayload
    {
        return ids
            .Where(kvStore.ContainsKey)
            .Select(id => kvStore.GetValueOrDefault(id))
            .Where(r => r != null)
            .Where(r => r!.Payload.GetType().IsAssignableFrom(typeof(T)))
            .Select(r => (T) r!.Payload);
    }


    public object? Get(string id)
    {
        return kvStore.GetValueOrDefault(id)?.Payload;
    }

    public bool IsIdExists(string id)
    {
        return kvStore.ContainsKey(id);
    }

    public T? Get<T>(string id)
    {
        var payload = Get(id);
        if (payload == null)
            return default;


        if (!payload.GetType().IsAssignableFrom(typeof(T)) && payload is not T)
            return default;

        return (T) payload;
    }

    public int Size()
    {
        return kvStore.Count;
    }

    public DateTime GetCreationDateTime()
    {
        var fi = new FileInfo(filePath);
        return fi.CreationTimeUtc;
    }

    public DateTime GetLastSaveDateTime()
    {
        var fi = new FileInfo(filePath);
        return fi.LastWriteTimeUtc;
    }

    public bool Remove(string id)
    {
        return kvStore.Remove(id, out _);
    }

    public IEnumerable<object?> Get(IEnumerable<string> ids)
    {
        return ids.Select(Get);
    }

    public void Clear()
    {
        kvStore.Clear();
    }

    public void Save()
    {
        Write(new StreamWriter(filePath));
    }

    public void Drop()
    {
        mutex.WaitOne();
        try
        {
            File.Delete(filePath);
        }
        finally
        {
            mutex.Close();
        }
    }

    private void Write(TextWriter writer)
    {
        mutex.WaitOne();
        try
        {
            var items = kvStore.Values.ToList();
            var serializer = new XmlSerializer(
                typeof(List<RegistryItem>),
                items.Select(r => r.Payload.GetType()).Distinct().ToArray()
            );

            var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.Indentation = 4;

            serializer.Serialize(xmlWriter, items);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private void Read(string xmlFilePath)
    {
        mutex.WaitOne();
        try
        {
            var reader = new StreamReader(xmlFilePath);
            
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(IRegistryPayload).IsAssignableFrom(t))
                .Concat(AdditionalTypes)
                .Distinct().ToArray();
    
            var serializer = new XmlSerializer(typeof(List<RegistryItem>), types);
            var items = (List<RegistryItem>) serializer.Deserialize(reader)!;
            foreach (var registryItem in items) kvStore[registryItem.Id] = registryItem;
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }


    public string DumpXml()
    {
        var sw = new StringWriter();
        Write(sw);
        sw.Flush();
        sw.Close();
        return sw.ToString();
    }

    private void Register(RegistryItem registryItem)
    {
        kvStore[registryItem.Id] = registryItem;
    }
}