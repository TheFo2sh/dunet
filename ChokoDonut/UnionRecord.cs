using System.Text;
using Microsoft.CodeAnalysis;

namespace ChokoDonut;

internal record UnionRecord(
    List<string> Imports,
    string? Namespace,
    Accessibility Accessibility,
    string Name,
    List<TypeParameter> TypeParameters,
    List<TypeParameterConstraint> TypeParameterConstraints,
    List<UnionRecordMember> Members,
    Stack<ParentType> ParentTypes
)
{
    // Extension methods cannot be generated for a union declared in a top level program (no namespace).
    // It also doesn't make sense to generate Match extensions if there are no members to match aginst.
    public bool SupportsAsyncMatchExtensionMethods() => Namespace is not null && Members.Count > 0;

    public string GetSourceText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine($"namespace {Namespace} {{");
        sb.AppendLine($"public interface I{Name}Handler");
        sb.AppendLine("{");
        foreach (var member in Members)
        {
            sb.AppendLine($"void Apply({Name}.{member.Name} obj);");
        }

        sb.AppendLine("}");
        sb.AppendLine("}");
        

        return sb.ToString();

    }

    public string GetExtensionsSourceText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine($"namespace {Namespace} {{");
        sb.AppendLine($"public static class {Name}HandlerExtensions");
        sb.AppendLine("{");

        sb.AppendLine($"public static void Apply(this I{Name}Handler context, {Name} obj)");
        sb.AppendLine("{");
        sb.AppendLine("switch (obj)");
        sb.AppendLine("{");
        foreach (var member in Members)
        {
            sb.AppendLine($"case {Name}.{member.Name} {member.Name.ToLowerInvariant()}:");
            sb.AppendLine($"context.Apply({member.Name.ToLowerInvariant()});");
            sb.AppendLine("break;");
        }

        sb.AppendLine("}");
        sb.AppendLine("}");
        
        sb.AppendLine("}");
        sb.AppendLine("}");
        return sb.ToString();
    }

}