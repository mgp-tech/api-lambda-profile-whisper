namespace Profile.Test.Unit.Infra.Repository.Base;

public class GetRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Get_Object()
    {
        Mock<ILogger<GetRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        GetRepositoryBase<EntityTest> getRepositoryBase =
            new(stub, Mock.Of<ILogger<GetRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));
        var entity2 = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));

        await stub.Set<EntityTest>().AddAsync(entity);
        await stub.Set<EntityTest>().AddAsync(entity2);
        await stub.SaveChangesAsync();

        var result = await getRepositoryBase.ExecuteAsync();
        result.Should().HaveCount(2);
    }
}