namespace Profile.Test.Unit.Infra.Repository.Base;

public class UpdateRepositoryTest
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_Update_Object()
    {
        Mock<ILogger<UpdateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        UpdateRepositoryBase<EntityTest> updateRepositoryBase =
            new(stub, Mock.Of<ILogger<UpdateRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));

        await stub.Set<EntityTest>().AddAsync(entity);
        await stub.SaveChangesAsync();

        entity.Name = "Test";
        await updateRepositoryBase.ExecuteAsync(entity);
        var result = await stub.SaveChangesAsync();
        result.Should().Be(1);
    }

    [Fact]
    public async Task Should_Not_Update_When_Not_Exists_Object()
    {
        Mock<ILogger<UpdateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        UpdateRepositoryBase<EntityTest> updateRepositoryBase =
            new(stub, Mock.Of<ILogger<UpdateRepositoryBase<EntityTest>>>());

        var guid = Guid.NewGuid();
        var entity = new EntityTest(guid, _faker.Random.String2(0, 100));

        var result = () => updateRepositoryBase.ExecuteAsync(entity);
        await result.Should().ThrowAsync<NotFoundException>().WithMessage($"No entity record found with id: {guid}");
    }

    [Fact]
    public async Task Should_Throw_RepositoryException_When_Update_Object_With_Id_Invalid()
    {
        Mock<ILogger<UpdateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        UpdateRepositoryBase<EntityTest> updateRepositoryBase =
            new(stub, Mock.Of<ILogger<UpdateRepositoryBase<EntityTest>>>());

        var entity = new EntityTest(Guid.Empty, _faker.Random.String2(0, 100));
        var result = () => updateRepositoryBase.ExecuteAsync(entity);

        await result.Should().ThrowAsync<RepositoryException>().WithMessage("EntityTest property Id can't be empty");
    }

    [Fact]
    public async Task Should_Throw_RepositoryException_When_Update_Object_Null()
    {
        Mock<ILogger<UpdateRepositoryBase<EntityTest>>> loggerRepositoryTest = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        DatabaseContextStub stub = new(options, Mock.Of<ILogger<DatabaseContext>>(), credentialMock.Object);

        UpdateRepositoryBase<EntityTest> updateRepositoryBase =
            new(stub, Mock.Of<ILogger<UpdateRepositoryBase<EntityTest>>>());

        var result = () => updateRepositoryBase.ExecuteAsync(null!);

        await result.Should().ThrowAsync<RepositoryException>().WithMessage("EntityTest can't be null");
    }
}