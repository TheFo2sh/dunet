namespace ChokoDonut;

internal record UnionRecordMember(
    string Name,
    List<TypeParameter> TypeParameters,
    List<RecordProperty> Properties
);