namespace Profile.Test.Unit.Infra.Repository.Base;

public class QueryRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Query_Object()
    {
        Mock<ILogger<QueryRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        QueryRepositoryBase<EntityTest> queryRepositoryBase =
            new(stub, Mock.Of<ILogger<QueryRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));
        var entity2 = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));

        await stub.Set<EntityTest>().AddAsync(entity);
        await stub.Set<EntityTest>().AddAsync(entity2);
        await stub.SaveChangesAsync();

        var result = await queryRepositoryBase.Query.FirstOrDefaultAsync(x => x.Id == entity.Id);
        result?.Id.Should().Be(entity.Id);
    }
}