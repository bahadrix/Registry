using System;
using System.IO;
using NimbleRegistry;
using NUnit.Framework;

namespace Tests;

public class MixedTests
{
    private TempFolder tempFolder;


    [SetUp]
    public void Setup()
    {
        tempFolder = new TempFolder("PanamaTest");
        TestContext.WriteLine("Test folder: " + tempFolder.Folder.FullName);
    }

    [Test]
    public void TestRegistry()
    {
        var factory = new XmlRegistryFactory(tempFolder.Folder.FullName);
        var store = new RegistryStore<XmlRegistry>(factory);

        var testRegistry = store.CreativeGet("test");

        var person = new Person
        {
            Name = "Jamal",
            Address = new Address
            {
                Street = "Panama St. 18",
                City = "Malibu Ct."
            }
        };
        testRegistry.Register(person);

        testRegistry.Save();

        File.WriteAllText(Path.Join(tempFolder.Folder.FullName, "test2.xml"), testRegistry.DumpXml());
        var store2 = new RegistryStore<XmlRegistry>(factory);
        var testRegistry2 = store2.CreativeGet("test2");

        Assert.AreEqual(testRegistry.DumpXml(), testRegistry2.DumpXml());
        
        Assert.NotNull(testRegistry2.Get<Person>(person.Id));
    }

    [TearDown]
    public void TearDown()
    {
        tempFolder.Dispose();
    }

    public class BaseEntity : IRegistryPayload
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
    
    public class Person : BaseEntity
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }
}