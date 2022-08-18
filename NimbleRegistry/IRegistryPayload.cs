namespace NimbleRegistry;

public interface IRegistryPayload
{
    public string Id { get; set; }
}

public static class RegistryItemPayloadExtensions
{
    public static string Register(this IRegistryPayload payload, IRegistry registry)
    {
        return registry.Register(payload);
    }
}