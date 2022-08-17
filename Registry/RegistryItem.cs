using System.Xml.Serialization;

namespace Registry;

public class RegistryItem
{
    public RegistryItem()
    {
        Id = Guid.NewGuid().ToString();
    }

    public RegistryItem(string id, object payload)
    {
        Id = id;
        Payload = payload;
    }

    [XmlAttribute] public string Id { get; set; }

    public object Payload { get; set; } = null!;
}