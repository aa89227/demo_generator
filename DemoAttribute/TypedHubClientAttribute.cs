namespace DemoAttribute;

[AttributeUsage(AttributeTargets.Class)]
public class TypedHubClientAttribute : Attribute
{
    public TypedHubClientAttribute(Type interfaceType)
    {
    }
}