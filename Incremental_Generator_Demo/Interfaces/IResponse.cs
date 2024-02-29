using Incremental_Generator_Demo.Models;

namespace Incremental_Generator_Demo.Interfaces;

public interface IResponse
{
    Task EventA(int a, string b);
    Task EventB(Data data);
}