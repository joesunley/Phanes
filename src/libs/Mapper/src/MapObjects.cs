namespace Phanes.Mapper;

public abstract class MapObject
{
	protected readonly Map _parent;
	
	public Guid Id { get; set; }

	protected MapObject(Map parent)
	{
		Id = Guid.NewGuid();
		
		_parent = parent;
	}

	protected MapObject(Map parent, Guid id)
	{
		Id = id;
		
		_parent = parent;
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

	public PointObject(Map parent, Colour innerColour, Colour outerColour, float innerRadius, float outerRadius) : base(parent)
	{
		InnerColour = innerColour;
		OuterColour = outerColour;
		InnerRadius = innerRadius;
		OuterRadius = outerRadius;
	}
	
	public PointObject(Map parent, Guid id, Colour innerColour, Colour outerColour, float innerRadius, float outerRadius) : base(parent, id)
	{
		InnerColour = innerColour;
		OuterColour = outerColour;
		InnerRadius = innerRadius;
		OuterRadius = outerRadius;
	}

	public override int ZIndex(int index)
	{
		return index switch
		{
			0 => _parent.Colours.GetZIndex(InnerColour),
			1 => _parent.Colours.GetZIndex(OuterColour),
			_ => -1,
		};
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
	
	public LineObject(Map parent, IEnumerable<vec2> points, float width, Colour colour) : base(parent)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
	}
	
	public LineObject(Map parent, Guid id, IEnumerable<vec2> points, float width, Colour colour) : base(parent, id)
	{
		Points = new(points);
		Width = width;
		Colour = colour;
	}

	public override int ZIndex(int index)
		=> _parent.Colours.GetZIndex(Colour);

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
	
	public Colour BorderColour { get; set; }
	
	public IFill Fill { get; set; }

	public AreaObject(Map parent, IEnumerable<vec2> points, float width, Colour borderColour, IFill fill) : base(parent)
	{
		Points = new(points);
		Width = width;
		BorderColour = borderColour;
		Fill = fill;
	}
	
	public AreaObject(Map parent, Guid id, IEnumerable<vec2> points, float width, Colour borderColour, IFill fill) : base(parent, id)
	{
		Points = new(points);
		Width = width;
		BorderColour = borderColour;
		Fill = fill;
	}

	public override int ZIndex(int index)
	{
		return index switch
		{
			0 => _parent.Colours.GetZIndex(BorderColour),
			1 => _parent.Colours.GetZIndex((SolidFill)Fill),
			_ => -1,
		};
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
	public TextObject(Map parent) : base(parent)
	{
		throw new NotImplementedException();
	}

	public TextObject(Map parent, Guid id) : base(parent, id)
	{
		throw new NotImplementedException();
	}
}