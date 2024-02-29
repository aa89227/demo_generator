using DemoAttirbute;
using Incremental_Generator_Demo.Interfaces;

Console.WriteLine(new TypedClient());

[TypedHubClient(typeof(IResponse))]
public partial class TypedClient
{
    
}