using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using Xunit;

namespace ChokoDonut.Test;

public class UnitTest1
{

    [Fact]
    public void Test1()
    {
        var manger = new Manger();
        var handler = new Handler(manger);
        Shape circle = new Shape.Circle(5);
        handler.Apply(circle);
        Assert.Equal("Circle",manger.Type);
        Assert.Equal(5,(manger.Context as Shape.Circle).Radius);
    }
}