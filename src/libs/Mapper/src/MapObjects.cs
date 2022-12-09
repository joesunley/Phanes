namespace Phanes.Mapper;

public abstract class MapObject
{
	public Guid Id { get; set; }

	protected MapObject()
	{
		Id = Guid.NewGuid();
	}

	protected MapObject(Guid id)
	{
		Id = id;
	}

	public virtual int ZIndex(int index) => -1;

	public virtual vec4 GetBoundingBox() => vec4.Zero;
}

public sealed class PointObject : MapObject
{
	public Colour InnerColour { get; set; }
	public Colour OuterColour { get; set; }
	
	public float InnerRadius { get; set; }
	public float OuterRadius { get; set; }

	public PointObject(Colour innerColour, Colour outerColour, float innerRadius, float outerRadius)
	{
		InnerColour = innerColour;
		OuterColour = outerColour;
		InnerRadius = innerRadius;
		OuterRadius = outerRadius;
	}
	
	public PointObject(Guid id, Colour innerColour, Colour outerColour, float innerRadius, float outerRadius) : base(id)
	{
		InnerColour = innerColour;
		OuterColour = outerColour;
		InnerRadius = innerRadius;
		OuterRadius = outerRadius;
	}

	public override vec4 GetBoundingBox()
	{
		float radius = InnerRadius + OuterRadius;

		vec2 topLeft = (-radius, -radius);
		vec2 bottomRight = (radius, radius);
		
		return new(topLeft, bottomRight);
	}
}

public sealed class LineObject : MapObject
{
	public List<vec2> Points { get; set; } // TODO: Make this support beziers
	
	public float Width { get; set; }
	
	public Colour Colour { get; set; }
	
	public LineObject(IEnumerable<vec2> points, float width, Colour colour)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
	}
	
	public LineObject(Guid id, IEnumerable<vec2> points, float width, Colour colour) : base(id)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
	}

	public override vec4 GetBoundingBox()
	{
		vec2 topLeft = vec2.MaxValue,
			 bottomRight = vec2.MinValue;
		
		foreach (vec2 point in Points)
		{
			topLeft = vec2.Min(topLeft, point);
			bottomRight = vec2.Max(bottomRight, point);
		}

		return new(topLeft, bottomRight);
	}
}

public sealed class AreaObject : MapObject
{
	public List<vec2> Points { get; set; } // TODO: Make this support beziers
	
	public float Width { get; set; }
	
	public Colour Colour { get; set; }
	
	public IFill Fill { get; set; }

	public AreaObject(IEnumerable<vec2> points, float width, Colour colour, IFill fill)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
		Fill = fill;
	}
	
	public AreaObject(Guid id, IEnumerable<vec2> points, float width, Colour colour, IFill fill) : base(id)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
		Fill = fill;
	}
	
	public override vec4 GetBoundingBox()
	{
		vec2 topLeft = vec2.MaxValue,
			 bottomRight = vec2.MinValue;

		foreach (vec2 point in Points)
		{
			topLeft = vec2.Min(topLeft, point);
			bottomRight = vec2.Max(bottomRight, point);
		}

		return new(topLeft, bottomRight);
	}
}

public sealed class TextObject : MapObject
{
	
}