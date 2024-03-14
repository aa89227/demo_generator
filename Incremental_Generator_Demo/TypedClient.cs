using DemoAttribute;
using Incremental_Generator_Demo.Interfaces;
using Incremental_Generator_Demo.Models;

namespace Incremental_Generator_Demo;

[TypedHubClient(typeof(IResponse))]
public partial class TypedClient
{
}

public class TargetClass
{
    public event EventADelegate? EventAHandler;
    public delegate void EventADelegate(int a, string b);
    public event EventBDelegate? EventBHandler;
    public delegate void EventBDelegate(Data data);
    public TargetClass(HubConnection hubConnection)
    {
        hubConnection.On<int, string>("EventA", (arg1, arg2) => EventAHandler?.Invoke(arg1, arg2));
        hubConnection.On<Data>("EventB", (arg1) => EventBHandler?.Invoke(arg1));
    }
}

public class HubConnection
{
    public void On<T>(string v, Action<T> action)
    {
        throw new NotImplementedException();
    }

    public void On<T1, T2>(string v, Action<T1, T2> action)
    {
        throw new NotImplementedException();
    }
}