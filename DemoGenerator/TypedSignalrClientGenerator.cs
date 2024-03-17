using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DemoGenerator;

[Generator(LanguageNames.CSharp)]
public class TypedSignalrClientGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // predict: node 是 class 且上面掛有 attribute TypeHubClientAttribute
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "DemoAttribute.TypedHubClientAttribute",
            predicate: static (node, _) => node.IsKind(SyntaxKind.ClassDeclaration),
            transform: static (ctx, _) => TransformContext(ctx)
        );

        context.RegisterSourceOutput(provider, static (productionContext, tuple) =>
        {
            productionContext.AddSource("TypedSignalrClientXXX.g.cs",
                $$"""
                  namespace Incremental_Generator_Demo;

                  public partial class {{tuple.Name}}
                  {
                  {{GenerateDelegates(tuple.Name, tuple.InterfaceType)}}
                  }
                  """);
        });
    }

    private static string GenerateDelegates(string className, INamespaceOrTypeSymbol tupleInterfaceType)
    {
        var delegateAndCtor = GenerateDelegatesAndCtorBody(tupleInterfaceType);
        return $$"""
               {{delegateAndCtor.eventAndDelegate}}
                
                   public {{className}}(HubConnection hubConnection)
                   {
                       {{delegateAndCtor.ctorBody}}
                   }
                
               """;
    }

    private static (string eventAndDelegate, string ctorBody) GenerateDelegatesAndCtorBody(INamespaceOrTypeSymbol tupleInterfaceType)
    {
        return tupleInterfaceType.GetMembers()
            .Where(x => x.Kind == SymbolKind.Method)
            .Select(x => (IMethodSymbol)x)
            .Select(x =>
            {
                var types = string.Join(",", x.Parameters.Select(p => p.Type));
                var parametersInDelegate = string.Join(", ", x.Parameters.Select(p => $"{p.Type} {p.Name}"));
                var parametersInOn = string.Join(", ", x.Parameters.Select(p => p.Name));
                var action = $"({parametersInOn}) => {x.Name}Handler?.Invoke({parametersInOn})";
                return (
                    $"""
                        public event {x.Name}Delegate? {x.Name}Handler;
                        public delegate void {x.Name}Delegate({parametersInDelegate});
                    
                    """,
                    $"hubConnection.On<{types}>(\"{x.Name}\", {action});\n\t\t");
            })
            .Aggregate((x, y) => (x.Item1 + y.Item1, x.Item2 + y.Item2));
    }

    private static (string Name, INamedTypeSymbol InterfaceType) TransformContext(GeneratorAttributeSyntaxContext ctx)
    {
        var typeSymbol = (INamedTypeSymbol)ctx.Attributes
            .First(x => x.AttributeClass?.Name == "TypedHubClientAttribute")
            .ConstructorArguments
            .First().Value!;
        return (ctx.TargetSymbol.Name, typeSymbol);
    }
}