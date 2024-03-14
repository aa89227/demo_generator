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
                     {{GenerateDelegates(tuple.InterfaceType)}}
                  }
                  """);
        });
    }

    private static string GenerateDelegates(INamedTypeSymbol tupleInterfaceType)
    {
        return $$"""
                 public static void Print()
                 {
                     Console.WriteLine("{{tupleInterfaceType.Name}}");
                 }
                 """;
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