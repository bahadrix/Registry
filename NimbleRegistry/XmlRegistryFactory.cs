namespace NimbleRegistry;

public class XmlRegistryFactory : IRegistryFactory<XmlRegistry>
{
    private readonly string folderPath;

    public XmlRegistryFactory(string folderPath)
    {
        this.folderPath = folderPath;
    }

    public XmlRegistry Create(string name)
    {
        return new XmlRegistry(Path.Join(folderPath, $"{name}.xml"));
    }

    public IEnumerable<string> Scan()
    {
        return Directory.GetFiles(folderPath, "*.xml")
            .Select(Path.GetFileNameWithoutExtension)!;
    }
}