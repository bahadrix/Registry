namespace NimbleRegistry;

public interface IRegistryFactory<out T> where T : IRegistry
{
    T Create(string name);
    bool Delete(string name);
}