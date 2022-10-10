using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ChokoDonut;

[Generator]
public class ChokoDonutGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var targets = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node.IsDecoratedRecord(),
                transform: static (ctx, _) => GetGenerationTarget(ctx)
            )
            .Where(static m => m is not null);

        var compilation = context.CompilationProvider.Combine(targets.Collect());

        context.RegisterSourceOutput(
            compilation,
            static (spc, source) => Execute(source.Left, source.Right!, spc)
        );
    }

    private static void Execute(Compilation compilation, ImmutableArray<RecordDeclarationSyntax?> recordDeclarations, SourceProductionContext context)
    {
        if (recordDeclarations.IsDefaultOrEmpty)
        {
            return;
        }

        var unionRecords = GetCodeToGenerate(
            compilation,
            recordDeclarations,
            context.CancellationToken
        );

        foreach (var unionRecord in unionRecords)
        {
            context.AddSource($"I{unionRecord.Name}Handler.g.cs",SourceText.From( unionRecord.GetSourceText(),Encoding.UTF8));
            context.AddSource($"I{unionRecord.Name}HandlerExtensions.g.cs",SourceText.From( unionRecord.GetExtensionsSourceText(),Encoding.UTF8));

        }
    }

    private static RecordDeclarationSyntax? GetGenerationTarget(GeneratorSyntaxContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;

        foreach (var attributeList in recordDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var attributeSymbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, attribute)
                    .Symbol?.ContainingType;

                if (attributeSymbol is null)
                {
                    continue;
                }

                var fullAttributeName = attributeSymbol.ToDisplayString();

                if (fullAttributeName is GenerateInterfaceAttributeSource.FullyQualifiedName)
                {
                    return recordDeclaration;
                }
            }
        }

        return null;
    }
    
    private static List<UnionRecord> GetCodeToGenerate(
        Compilation compilation,
        IEnumerable<RecordDeclarationSyntax> recordDeclarations,
        CancellationToken _
    )
    {
        var unionRecords = new List<UnionRecord>();

        foreach (var recordDeclaration in recordDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(recordDeclaration.SyntaxTree);
            var recordSymbol = semanticModel.GetDeclaredSymbol( recordDeclaration);

            if (recordSymbol is null)
            {
                continue;
            }

            var imports = recordDeclaration
                .GetImports()
                .Where(static import => !import.IsImporting("ChokoDunet"))
                .Select(static import => import.ToString());
            var @namespace = recordSymbol.GetNamespace();
            var unionRecordTypeParameters = recordDeclaration.TypeParameterList?.Parameters.Select(
                static typeParam => new TypeParameter(typeParam.Identifier.ToString())
            );
            var unionRecordTypeParameterConstraints = recordDeclaration.ConstraintClauses.Select(
                constraint => new TypeParameterConstraint(constraint.ToString())
            );
            var unionRecordMemberDeclarations = recordDeclaration
                .DescendantNodes()
                .Where(static node => node.IsKind(SyntaxKind.RecordDeclaration))
                .OfType<RecordDeclarationSyntax>();

            var unionRecordMembers = new List<UnionRecordMember>();

            foreach (var memberRecordDeclaration in unionRecordMemberDeclarations)
            {
                var typeParameters = memberRecordDeclaration.TypeParameterList?.Parameters
                    .Select(static typeParam => typeParam.Identifier.ToString())
                    .Select(static identifier => new TypeParameter(identifier));
                var properties = memberRecordDeclaration.ParameterList?.Parameters.Select(
                    static parameter =>
                        new RecordProperty(
                            Type: parameter.Type?.ToString() ?? "",
                            Name: parameter.Identifier.ToString()
                        )
                );
                var memberRecord = new UnionRecordMember(
                    Name: memberRecordDeclaration.Identifier.ToString(),
                    TypeParameters: typeParameters?.ToList() ?? new(),
                    Properties: properties?.ToList() ?? new()
                );

                unionRecordMembers.Add(memberRecord);
            }

            var record = new UnionRecord(
                Imports: imports.ToList(),
                Namespace: @namespace,
                Accessibility: recordSymbol.DeclaredAccessibility,
                Name: recordSymbol.Name,
                TypeParameters: unionRecordTypeParameters?.ToList() ?? new(),
                TypeParameterConstraints: unionRecordTypeParameterConstraints?.ToList() ?? new(),
                Members: unionRecordMembers,
                ParentTypes: GetParentTypes(semanticModel, recordDeclaration)
            );

            unionRecords.Add(record);
        }

        return unionRecords;
    }

    private static Stack<ParentType> GetParentTypes(
        SemanticModel semanticModel,
        RecordDeclarationSyntax recordDeclaration
    )
    {
        var parentTypes = new Stack<ParentType>();

        RecursivelyAddParentTypes(semanticModel, recordDeclaration, parentTypes);

        return parentTypes;
    }

    private static void RecursivelyAddParentTypes(
        SemanticModel semanticModel,
        SyntaxNode declaration,
        Stack<ParentType> parentTypes
    )
    {
        var parent = declaration.Parent;

        if (parent is null)
        {
            return;
        }

        if (!parent.IsClassOrRecordDeclaration())
        {
            return;
        }

        var parentSymbol = semanticModel.GetDeclaredSymbol(parent);

        // Ignore top level statement synthentic program class.
        if (parentSymbol?.ToDisplayString() is null or "<top-level-statements-entry-point>")
        {
            return;
        }

        var parentDeclaration = (TypeDeclarationSyntax)parent;

        // We can only declare a nested union type within a partial parent type declaration.
        if (!parentDeclaration.IsPartial())
        {
            return;
        }

        var parentType = new ParentType(
            IsRecord: parent.IsRecordDeclaration(),
            Name: parentSymbol.Name
        );

        parentTypes.Push(parentType);

        RecursivelyAddParentTypes(semanticModel, parent, parentTypes);
    }
}