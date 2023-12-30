namespace Profile.Test.Unit.Infra.Context;

public class DatabaseContextTest
{
    [Fact]
    public void Should_Create_Connection_In_Memory()
    {
        Mock<ILogger<DatabaseContext>> logger = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        var context = new DatabaseContext(options, logger.Object, credentialMock.Object);
        context.Should().NotBeNull();
        context.Database.CanConnect().Should().BeTrue();
    }

    [Fact]
    public void Should_Not_Connect_Local_Database_In_Unit_Test()
    {
        Mock<ILogger<DatabaseContext>> logger = new();

        var options = new DbContextOptionsBuilder<DatabaseContext>().Options;

        Mock<ICredential> credentialMock = new();
        credentialMock.Setup(x => x.ExecuteAsync())
            .ReturnsAsync("Server=localhost;Database=local;Uid=root;Pwd=123;CharSet=utf8");

        var context = new DatabaseContext(options, logger.Object, credentialMock.Object);

        context.Should().NotBeNull();
        var exception = () => context.Database.CanConnect();
        exception.Should().Throw<MySqlException>();
    }
}