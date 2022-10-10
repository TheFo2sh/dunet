using ChokoDunet;

namespace ChokoDonut.Test;

[GenerateVoidHandlerInterfaceAttribute]
public record Shape(string Type)
{
    public record Circle(double Radius):Shape("Circle");
    public record Rectangle(double Width, double Height):Shape("Rectangle");
}