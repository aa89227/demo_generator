using Microsoft.CodeAnalysis;
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
            predicate: (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
            {
                return (ctx.TargetNode, ctx.Attributes);
            });
        
        context.RegisterSourceOutput(provider, static (productionContext, tuple) =>
        {
            if (tuple.TargetNode is not ClassDeclarationSyntax classDeclaration)
            {
                return;
            }
        });
    }
}