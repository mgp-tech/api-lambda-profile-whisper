namespace Profile.Test.Unit.Core.IoC;

public class IoCTest
{
    private ServiceCollection _serviceCollection = new();

    public IoCTest()
    {
        _serviceCollection.Clear();
        _serviceCollection.RegisterDependencies();
    }

    public static IEnumerable<object[]> DoubleTest =>
        new List<object[]>
        {
            new object[] { typeof(ICredential), typeof(Credential) },
            new object[] { typeof(ICreateRepositoryBase<>), typeof(CreateRepositoryBase<>) },
            new object[] { typeof(IUpdateRepositoryBase<>), typeof(UpdateRepositoryBase<>) },
            new object[] { typeof(IDeleteRepositoryBase<>), typeof(DeleteRepositoryBase<>) },
            new object[] { typeof(IGetRepositoryBase<>), typeof(GetRepositoryBase<>) },
            new object[] { typeof(IGetByIdRepositoryBase<>), typeof(GetByIdRepositoryBase<>) },
            new object[] { typeof(ISecretClient), typeof(SecretClient) },
            new object[] { typeof(IUnitOfWork), typeof(UnitOfWork) }
        };

    public static IEnumerable<object[]> SingleTest =>
        new List<object[]>
        {
            new object[] { typeof(DatabaseContext) },
            new object[] { typeof(IAmazonSecretsManager) }
        };

    [Theory]
    [MemberData(nameof(DoubleTest))]
    public void Should_Add_Interfaces_And_Implementations_In_Container(Type abstraction, Type implementation)
    {
        _serviceCollection.Any(x => x.ServiceType == abstraction).Should().BeTrue();
        _serviceCollection.Any(x => x.ImplementationType == implementation).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SingleTest))]
    public void Should_Add_Interfaces_In_Container(Type abstraction)
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.RegisterDependencies();
        serviceCollection.Any(x => x.ServiceType == abstraction).Should().BeTrue();
    }
}