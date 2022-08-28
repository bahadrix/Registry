namespace NimbleRegistry;

public class XmlRegistryFactory : IRegistryFactory<XmlRegistry>
{
    private readonly string folderPath;
    private readonly IEnumerable<Type> additionalTypes;
    
    public XmlRegistryFactory(string folderPath, IEnumerable<Type>? additionalTypes = null)
    {
        this.additionalTypes = additionalTypes ?? new List<Type>();
        this.folderPath = folderPath;
    }

    public XmlRegistry Create(string name)
    {
        var reg = new XmlRegistry(Path.Join(folderPath, $"{name}.xml"));
        foreach (var additionalType in additionalTypes)
        {
            reg.AdditionalTypes.Add(additionalType);
        }
        return reg;
    }

    public bool Delete(string name)
    {
        var path = Path.Join(folderPath, $"{name}.xml");
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;

    }

    public IEnumerable<string> Scan()
    {
        return Directory.GetFiles(folderPath, "*.xml")
            .Select(Path.GetFileNameWithoutExtension)!;
    }
}