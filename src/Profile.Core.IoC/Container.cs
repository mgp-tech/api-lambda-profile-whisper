namespace Profile.Core.IoC;

public static class Container
{
    public static void RegisterDependencies(this IServiceCollection service)
    {
        service.AddScoped<ICredential, Credential>();
        service.AddScoped<ISecretClient, SecretClient>();
        service.AddScoped<IUnitOfWork, UnitOfWork>();
        service.AddDbContext<DatabaseContext>();
        service.AddAWSService<IAmazonSecretsManager>();
        RegisterRepository(service);
    }

    [ExcludeFromCodeCoverage]
    private static void RegisterRepository(this IServiceCollection service)
    {
        var dictionary = new Dictionary<Type, Type>
        {
            { typeof(ICreateRepositoryBase<>), typeof(CreateRepositoryBase<>) },
            { typeof(IUpdateRepositoryBase<>), typeof(UpdateRepositoryBase<>) },
            { typeof(IDeleteRepositoryBase<>), typeof(DeleteRepositoryBase<>) },
            { typeof(IGetRepositoryBase<>), typeof(GetRepositoryBase<>) },
            { typeof(IGetByIdRepositoryBase<>), typeof(GetByIdRepositoryBase<>) }
        };

        foreach (var type in dictionary)
        {
            service.AddScoped(type.Key, type.Value);

            var abstractTypes = Assembly.GetAssembly(type.Key)?.GetTypes()
                ?.Where(x => x.IsInterface && x.GetInterface(type.Key.Name) != null).ToList();

            if (abstractTypes != null && abstractTypes.Any())
                foreach (var abstractType in abstractTypes)
                {
                    var concrete = Assembly.GetAssembly(type.Value)?.GetTypes()
                        .Where(x => x.IsClass && x.GetInterface(abstractType.Name) != null)?.FirstOrDefault();

                    if (concrete is not null)
                        service.AddScoped(abstractType, concrete);
                }
        }
    }
}