namespace NimbleRegistry;

public interface IRegistryItem
{
    string Id { get; set; }
    object Payload { get; set; }
}