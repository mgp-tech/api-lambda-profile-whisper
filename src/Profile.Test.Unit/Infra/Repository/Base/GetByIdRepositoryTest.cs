namespace Profile.Test.Unit.Infra.Repository.Base;

public class GetByIdRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_GetById_Object()
    {
        Mock<ILogger<GetByIdRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        GetByIdRepositoryBase<EntityTest> getByIdRepositoryBase =
            new(stub, Mock.Of<ILogger<GetByIdRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));

        await stub.Set<EntityTest>().AddAsync(entity);
        await stub.SaveChangesAsync();

        var result = await getByIdRepositoryBase.ExecuteAsync(entity.Id);
        result.Should().NotBeNull();
        result?.Id.Should().Be(entity.Id);
    }
}