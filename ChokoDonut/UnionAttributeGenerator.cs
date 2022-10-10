using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ChokoDonut;
[Generator]
public class UnionAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            ctx =>
                ctx.AddSource(
                    $"{GenerateInterfaceAttributeSource.Name}.g.cs",
                    SourceText.From(GenerateInterfaceAttributeSource.SourceCode, Encoding.UTF8)
                )
        );
    }
}