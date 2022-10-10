namespace ChokoDonut;

/// <summary>
/// Represents a parent type declaration that nests a union record.
/// </summary>
/// <param name="IsRecord">Whether the type is a record or a plain class.</param>
/// <param name="Name">This type's name.</param>
internal sealed record ParentType(bool IsRecord, string Name)
{
    public sealed override string ToString() => Name;
}