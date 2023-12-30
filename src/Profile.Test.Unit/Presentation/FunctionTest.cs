namespace Profile.Test.Unit.Presentation;

public class FunctionTest
{
    [Fact]
    public void Should_Return_String()
    {
        Function function = new();
        var result = function.FunctionHandler("test", Mock.Of<ILambdaContext>());
        result.Should().Be("TEST");
    }
}