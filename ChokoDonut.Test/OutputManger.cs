namespace ChokoDonut.Test;

public class OutputManger
{
    public object Context { get; private set; }
    public string Type { get; private set; }

    public void SetType(string type,object obj)
    {
        Type = type;
        Context = obj;
    }
}