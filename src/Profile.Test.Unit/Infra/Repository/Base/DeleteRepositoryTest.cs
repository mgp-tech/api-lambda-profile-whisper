namespace Profile.Test.Unit.Infra.Repository.Base;

public class DeleteRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Delete_Object()
    {
        Mock<ILogger<DeleteRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);
        DeleteRepositoryBase<EntityTest> deleteRepositoryBase =
            new(stub, Mock.Of<ILogger<DeleteRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));

        await stub.Set<EntityTest>().AddAsync(entity);
        await stub.SaveChangesAsync();

        await deleteRepositoryBase.ExecuteAsync(entity.Id);
        var result = await stub.SaveChangesAsync();
        result.Should().Be(1);
    }

    [Fact]
    public async Task Should_Not_Delete_When_Not_Exists_Object()
    {
        Mock<ILogger<DeleteRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);
        DeleteRepositoryBase<EntityTest> deleteRepositoryBase =
            new(stub, Mock.Of<ILogger<DeleteRepositoryBase<EntityTest>>>());

        var guid = Guid.NewGuid();
        var entity = new EntityTest(guid, _faker.Random.String2(0, 100));

        var result = () => deleteRepositoryBase.ExecuteAsync(entity.Id);
        await result.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Record not exists in database with id: {guid}");
    }
}