namespace ChokoDonut;

internal class GenerateInterfaceAttributeSource
{
    public const string Namespace = "ChokoDunet";
    public const string Name = "GenerateVoidHandlerInterfaceAttribute";
    public const string FullyQualifiedName = $"{Namespace}.{Name}";

    public const string SourceCode =
        @"using System;

namespace ChokoDunet;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateVoidHandlerInterfaceAttribute : Attribute {}";
}