namespace ChokoDonut;

internal record TypeParameter(string Name)
{
    public sealed override string ToString() => Name;
}