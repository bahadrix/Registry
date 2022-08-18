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

    public IEnumerable<string> Scan()
    {
        return Directory.GetFiles(folderPath, "*.xml")
            .Select(Path.GetFileNameWithoutExtension)!;
    }
}