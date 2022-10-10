namespace ChokoDonut;

/// <summary>
/// Represents a type parameter constraint on a union record's type parameters.
/// </summary>
/// <param name="Value">The full string value of the constraint. Ex: `where T : notnull, System.Exception`</param>
internal sealed record TypeParameterConstraint(string Value)
{
    public sealed override string ToString() => Value;
}