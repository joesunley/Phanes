using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace Phanes.Mapper;

public interface IFill {}

public sealed class SolidFill : IFill
{
	public Colour Colour { get; set; }

	public SolidFill(Colour colour)
	{
		Colour = colour;
	}

	public static implicit operator SolidFill(Colour colour)
		=> new(colour);
	public static implicit operator Colour(SolidFill fill)
		=> fill.Colour;
}

public sealed class ObjectFill : IFill
{
	public List<MapObject> Objects { get; set; }
	
	public vec2 Spacing { get; set; }
	
	public vec2 Offset { get; set; }

	public ObjectFill(IEnumerable<MapObject> mapObjects, vec2 spacing, vec2 offset)
	{
		Objects = new(mapObjects);
		
		Spacing = spacing;
		Offset = offset;
	}
}