namespace Profile.Test.Unit.Infra.Repository.Base;

public class CreateRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Create_New_Object()
    {
        Mock<ILogger<CreateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        CreateRepositoryBase<EntityTest> createRepositoryBase =
            new(stub, Mock.Of<ILogger<CreateRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));
        await createRepositoryBase.ExecuteAsync(entity);

        entity.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Throw_RepositoryException_When_Create_New_Object_With_Id_Invalid()
    {
        Mock<ILogger<CreateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        CreateRepositoryBase<EntityTest> createRepositoryBase =
            new(stub, Mock.Of<ILogger<CreateRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.NewGuid(), _faker.Random.String2(0, 100));
        var result = () => createRepositoryBase.ExecuteAsync(entity);

        await result.Should().ThrowAsync<RepositoryException>().WithMessage("EntityTest property Id must be empty");
    }

    [Fact]
    public async Task Should_Throw_RepositoryException_When_Create_New_Object_Null()
    {
        Mock<ILogger<CreateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        CreateRepositoryBase<EntityTest> createRepositoryBase =
            new(stub, Mock.Of<ILogger<CreateRepositoryBase<EntityTest>>>());

        var result = () => createRepositoryBase.ExecuteAsync(null!);

        await result.Should().ThrowAsync<RepositoryException>().WithMessage("EntityTest can't be null");
    }
}