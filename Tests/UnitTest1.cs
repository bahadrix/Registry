using System.IO;
using NimbleRegistry;
using NUnit.Framework;

namespace Tests;

public class MixedTests
{
    private TempFolder tempFolder;
    private string sampleXML = @"<?xml version=""1.0"" encoding=""utf-8""?>
    <ArrayOfRegistryItem xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <RegistryItem Id=""2d7b9f57-83a6-490c-9866-e9e5789421e0"">
        <Payload xsi:type=""Person"">
    <Id>2d7b9f57-83a6-490c-9866-e9e5789421e0</Id>
    <Name>Jamal</Name>
    <Address>
    <Street>Panama St. 18</Street>
    <City>Malibu Ct.</City>
    </Address>
    </Payload>
    </RegistryItem>
    </ArrayOfRegistryItem>";
    
    [SetUp]
    public void Setup()
    {
        
        tempFolder = new TempFolder("PanamaTest");
        TestContext.WriteLine("Test folder: " + tempFolder.Folder.FullName);
    }

    [Test]
    public void TestKundikness()
    {
        var testFile = Path.Join(tempFolder.Folder.FullName, "test2.xml");
        
        File.WriteAllText(testFile, sampleXML);
        
        var xmlRegistry = new XmlRegistry(testFile);

        var person = xmlRegistry.Get<Person>("2d7b9f57-83a6-490c-9866-e9e5789421e0");
        
        Assert.NotNull(person);

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
}