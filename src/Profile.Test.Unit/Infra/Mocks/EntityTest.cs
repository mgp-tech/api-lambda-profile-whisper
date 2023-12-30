namespace Profile.Test.Unit.Infra.Mocks;

[ExcludeFromCodeCoverage]
public class EntityTest : BaseEntity
{
    public EntityTest(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; set; }
}