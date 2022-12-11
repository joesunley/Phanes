namespace Phanes.Mapper;

public sealed class TextSymbol : Symbol
{
	public Font Font { get; set; }
	
	public Colour Colour { get; set; }
	
	public bool IsRotatable { get; set; }
	
	public SolidFill Fill { get; set; }
	
	public Colour BorderColour { get; set; }
	public float BorderWidth { get; set; }
	
	public Colour FramingColour { get; set; }
	public float FramingWidth { get; set; }

	public TextSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, Font font, Colour colour, bool isRotatable, SolidFill fill, Colour borderColour, float borderWidth, Colour framingColour, float framingWidth) 
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		Font = font;
		Colour = colour;
		IsRotatable = isRotatable;
		Fill = fill;
		BorderColour = borderColour;
		BorderWidth = borderWidth;
		FramingColour = framingColour;
		FramingWidth = framingWidth;
	}
	public TextSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, Font font, Colour colour, bool isRotatable, SolidFill fill, Colour borderColour, float borderWidth, Colour framingColour, float framingWidth) 
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		Font = font;
		Colour = colour;
		IsRotatable = isRotatable;
		Fill = fill;
		BorderColour = borderColour;
		BorderWidth = borderWidth;
		FramingColour = framingColour;
		FramingWidth = framingWidth;
	}
}