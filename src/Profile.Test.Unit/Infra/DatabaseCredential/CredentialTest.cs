namespace Profile.Test.Unit.Infra.DatabaseCredential;

public class CredentialTest
{
    public CredentialTest()
    {
        Environment.SetEnvironmentVariable("database-secret", "database-secret");
    }

    [Fact]
    public async Task Should_Get_Database_Credential()
    {
        Mock<ISecretClient> secretClientMock = new();
        secretClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                { "host", "localhost" },
                { "password", "123" },
                { "database", "local" },
                { "user", "root" }
            }));

        Credential credential = new(secretClientMock.Object, Mock.Of<ILogger<Credential>>());
        var result = await credential.ExecuteAsync();
        result.Should().NotBeNull();
        result.Should().ContainAll("localhost", "123", "local", "root");
        result.Should().Be("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");
        secretClientMock.Verify(
            x => x.GetAsync(It.Is<string>(key => key == Environment.GetEnvironmentVariable("database-secret"))),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Not_Found_When_Value_Not_Exists()
    {
        Mock<ISecretClient> secretClientMock = new();
        secretClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                { "host", "localhost" },
                { "password", "123" },
                { "database", "local" }
            }));

        Credential credential = new(secretClientMock.Object, Mock.Of<ILogger<Credential>>());
        var func = () => credential.ExecuteAsync();
        await func.Should().ThrowAsync<NotFoundException>().WithMessage("Secret key not exists!");
    }

    [Fact]
    public async Task Should_Throw_Not_Found_When_Key_Not_Exists()
    {
        Mock<ISecretClient> secretClientMock = new();
        secretClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                { "host", "localhost" },
                { "password", "123" },
                { "database", "local" },
                { "user", "" }
            }));

        Credential credential = new(secretClientMock.Object, Mock.Of<ILogger<Credential>>());
        var func = () => credential.ExecuteAsync();
        await func.Should().ThrowAsync<NotFoundException>().WithMessage("Not found secret value!");
    }

    [Fact]
    public async Task Should_Throw_Not_Found_When_Environment_Variable_Not_Exists()
    {
        Environment.SetEnvironmentVariable("database-secret", string.Empty);

        Mock<ISecretClient> secretClientMock = new();
        secretClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                { "host", "localhost" },
                { "password", "123" },
                { "database", "local" },
                { "user", "root" }
            }));

        Credential credential = new(secretClientMock.Object, Mock.Of<ILogger<Credential>>());
        var func = () => credential.ExecuteAsync();
        await func.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Environment variable not found: database-secret");
    }

    [Fact]
    public async Task Should_Throw_Invalid_Cast_When_To_Get_Database_Credential()
    {
        Mock<ISecretClient> secretClientMock = new();

        secretClientMock.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        Credential credential = new(secretClientMock.Object, Mock.Of<ILogger<Credential>>());

        var func = () => credential.ExecuteAsync();
        await func.Should().ThrowAsync<InvalidCastException>().WithMessage(
            "It wasn't possible to convert the secrets into a dictionary! The secrets were null or empty!");

        secretClientMock.Verify(
            x => x.GetAsync(It.Is<string>(key => key == Environment.GetEnvironmentVariable("database-secret"))),
            Times.Once);
    }
}