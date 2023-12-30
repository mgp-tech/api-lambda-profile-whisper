using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Profile.Test.Unit.Infra.Uow;

public class UnitOfWorkTest
{
    [Fact]
    public async Task Should_Commit_Transaction_Resilient()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Mock<ILogger<DatabaseContext>> loggerMock = new();
        Mock<ILogger<UnitOfWork>> loggerUowMock = new();
        Mock<ICredential> credentialMock = new();

        Mock<DatabaseContext> databaseContextMock = new(options, loggerMock.Object, credentialMock.Object);

        databaseContextMock.SetupGet(x => x.Database)
            .Returns(new DatabaseFacade(databaseContextMock.Object));

        databaseContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        UnitOfWork uow = new(databaseContextMock.Object, loggerUowMock.Object);
        await uow.CommitAsync();

        databaseContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        databaseContextMock.Verify(x => x.Database, Times.Exactly(2));
        loggerUowMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, type) =>
                    @object.ToString() == "Occurred an error when try to commit transaction: Error" &&
                    type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Commit_Transaction()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Mock<ILogger<DatabaseContext>> loggerMock = new();
        Mock<ILogger<UnitOfWork>> loggerUowMock = new();
        Mock<ICredential> credentialMock = new();

        Mock<DatabaseContext> databaseContextMock = new(options, loggerMock.Object, credentialMock.Object);

        databaseContextMock.SetupGet(x => x.Database)
            .Returns(new DatabaseFacade(databaseContextMock.Object));

        databaseContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Error"));

        UnitOfWork uow = new(databaseContextMock.Object, loggerUowMock.Object);
        await uow.CommitAsync();

        loggerUowMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, type) =>
                    @object.ToString() == "Occurred an error when try to commit transaction: Error" &&
                    type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}