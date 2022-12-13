using Phanes.Mapper;

namespace Phanes.Common.Renderer;

// Simplified clones of System.Windows.Shapes types for cross-platform usage

public interface IShape
{
    vec2 TopLeft { get; set; }
    float Opacity { get; set; }
    int ZIndex { get; set; }
}

public struct Ellipse : IShape
{
    public vec2 TopLeft { get; set; }

    public float Opacity { get; set; }

    public vec2 Size { get; set; }

    public uint Fill { get; set; }

    public float BorderWidth { get; set; }
    public uint BorderColour { get; set; }

    public IEnumerable<double> DashArray { get; set; }

    public int ZIndex { get; set; }

    public Ellipse()
    {
        TopLeft = vec2.Zero;

        Opacity = 1f;

        Size = vec2.Zero;

        Fill = Colour.Transparent;

        BorderWidth = 0f;
        BorderColour = Colour.Transparent;

        DashArray = Enumerable.Empty<double>();

        ZIndex = 0;
    }
}

public struct Line : IShape
{
    public vec2 TopLeft { get; set; }

    public float Opacity { get; set; }

    public List<vec2> Points { get; set; }

    public float Width { get; set; }
    public uint Colour { get; set; }

    public IEnumerable<double> DashArray { get; set; }

    public int ZIndex { get; set; }

    public Line()
    {
        TopLeft = vec2.Zero;

        Opacity = 1f;

        Points = new();

        Width = 0f;
        Colour = Phanes.Mapper.Colour.Transparent;

        DashArray = Enumerable.Empty<double>();

        ZIndex = 0;
    }
}

public struct Area : IShape
{
    public vec2 TopLeft { get; set; }

    public float Opacity { get; set; }

    public List<vec2> Points { get; set; }

    public uint Fill { get; set; }

    public bool IsClosed { get; set; }
    public float BorderWidth { get; set; }
    public uint BorderColour { get; set; }

    public IEnumerable<double> DashArray { get; set; }

    public int ZIndex { get; set; }

    public Area()
    {
        TopLeft = vec2.Zero;

        Opacity = 1f;

        Points = new();

        Fill = Colour.Transparent;

        IsClosed = true;
        BorderWidth = 0f;
        BorderColour = Colour.Transparent;

        DashArray = Enumerable.Empty<double>();

        ZIndex = 0;
    }
}

public struct Path : IShape
{ 
    public vec2 TopLeft { get; set; }

    public float Opacity { get; set; }

    //public vec2 StartPoint { get; set; }

    public List<IPathSegment> Segments { get; set; }

    public uint Fill { get; set; }

    public bool IsClosed { get; set; }
    public float BorderWidth { get; set; }
    public uint BorderColour { get; set; }

    public IEnumerable<double> DashArray { get; set; }

    public int ZIndex { get; set; }

    public Path()
    {
        TopLeft = vec2.Zero;

        Opacity = 1f;

        //StartPoint = vec2.Zero;

        Segments = new();

        Fill = Colour.Transparent;

        IsClosed = true;
        BorderWidth = 0f;
        BorderColour = Colour.Transparent;

        DashArray = Enumerable.Empty<double>();

        ZIndex = 0;
    }
}

public struct Rectangle : IShape
{
    public vec2 TopLeft { get; set; }

    public float Opacity { get; set; }

    public vec2 Size { get; set; }

    public uint Fill { get; set; }

    public float BorderWidth { get; set; }
    public uint BorderColour { get; set; }

    public IEnumerable<double> DashArray { get; set; }

    public int ZIndex { get; set; }

    public Rectangle()
    {
        TopLeft = vec2.Zero;

        Opacity = 1f;

        Size = vec2.Zero;

        Fill = Colour.Transparent;

        BorderWidth = 0f;
        BorderColour = Colour.Transparent;

        DashArray = Enumerable.Empty<double>();

        ZIndex = 0;
    }
}

public interface IPathSegment { }

public struct PolyBezierSegment : IPathSegment
{
    public List<BezierPoint> Points { get; set; }
}

public struct PolyLineSegment : IPathSegment
{
    public vec2 StartPoint { get; set; }

    public List<vec2> Points { get; set; }
}

public struct Text : IShape
{
    public vec2 TopLeft { get; set; }
    
    public string Content { get; set; }
    
    public Font Font { get; set; }
    
    public (HorizontalAlignment h, VerticalAlignment v) Alignment { get; set; }
    
    public (Colour colour, float width) Border { get; set; }
    
    public (Colour colour, float width) Framing { get; set; }
    
    public Colour Fill { get; set; }
    
    public float Opacity { get; set; }
    
    public int ZIndex { get; set; }
}