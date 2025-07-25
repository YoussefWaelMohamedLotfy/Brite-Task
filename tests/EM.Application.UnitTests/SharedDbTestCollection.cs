namespace EM.Application.UnitTests;

[CollectionDefinition("InMemoryDb")]
public class SharedDbTestCollection : ICollectionFixture<InMemoryDbProvider>;
