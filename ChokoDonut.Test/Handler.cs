namespace ChokoDonut.Test;

public class Handler:IShapeHandler
{
    private readonly Manger _manger;

    public Handler(Manger manger)
    {
        _manger = manger;
    }
    public void Apply(Shape.Circle obj)
    {
        _manger.SetType("Circle",obj);
    }

    public void Apply(Shape.Rectangle obj)
    {
        _manger.SetType("Rectangle",obj);
    }
}