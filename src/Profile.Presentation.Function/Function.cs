[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Profile.Presentation.Function;

public class Function
{
    private readonly ServiceProvider _provider;

    public Function()
    {
        var service = new ServiceCollection();
        service.RegisterDependencies();
        _provider = service.BuildServiceProvider();
    }

    public string FunctionHandler(string input, ILambdaContext context)
    {
        var logger = _provider.GetService<ILogger<Function>>();
        logger?.LogInformation("Initialize function");

        return input.ToUpper();
    }
}