namespace Profile.Test.Unit.Infra.Secret;

public class SecretClientTest
{
    [Fact]
    public async Task Should_Get_A_Secret()
    {
        const string nameCredential = "credentials-database-rds";

        Mock<IAmazonSecretsManager> secretsMock = new();
        secretsMock.Setup(x =>
                x.GetSecretValueAsync(It.Is<GetSecretValueRequest>(request => request.SecretId == nameCredential),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSecretValueResponse()
            {
                Name = nameCredential,
                SecretString = JsonConvert.SerializeObject(new Dictionary<string, string>()
                {
                    { "host", "localhost" },
                    { "password", "123" },
                    { "database", "local" }
                })
            }).Verifiable();

        SecretClient secretClient = new(secretsMock.Object, Mock.Of<ILogger<SecretClient>>());
        var result = await secretClient.GetAsync(nameCredential);
        var secrets = JsonConvert.DeserializeObject<Dictionary<string, string>>(result as string ?? string.Empty);

        secrets.Should().ContainKeys("host", "password", "database");
        secrets.Should().ContainValues("localhost", "123", "local");
        secretsMock.Verify(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Name_Is_Invalid()
    {
        Mock<IAmazonSecretsManager> secretsMock = new();
        SecretClient secretClient = new(secretsMock.Object, Mock.Of<ILogger<SecretClient>>());
        var result = () => secretClient.GetAsync(null!);

        await result.Should().ThrowAsync<SecretClientException>().WithMessage("Secrets name can't be null or empty");

        secretsMock.Verify(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Not_Exists_Secret()
    {
        Mock<IAmazonSecretsManager> secretsMock = new();
        const string nameCredential = "credentials-database-rds";

        secretsMock.Setup(x => x.GetSecretValueAsync(It.IsAny<GetSecretValueRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as GetSecretValueResponse).Verifiable();

        SecretClient secretClient = new(secretsMock.Object, Mock.Of<ILogger<SecretClient>>());
        var result = () => secretClient.GetAsync(nameCredential);

        await result.Should().ThrowAsync<NotFoundException>().WithMessage("The secret couldn't be found");

        secretsMock.Verify(
            x => x.GetSecretValueAsync(It.Is<GetSecretValueRequest>(request => request.SecretId == nameCredential),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}