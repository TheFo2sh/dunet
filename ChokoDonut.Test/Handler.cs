using ChokoDunet;

namespace ChokoDonut.Test;

public class Handler:IShapeHandler
{
    private readonly OutputManger _manger;

    public Handler(OutputManger manger)
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