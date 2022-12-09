namespace Phanes.Mapper;

public sealed class LineSymbol : Symbol, IPathSymbol
{
	public Colour Colour { get; set; }
	
	public float Width { get; set; }
	
	public DashStyle DashStyle { get; set; }
	
	public MidStyle MidStyle { get; set; }

	public LineSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, Colour colour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		Colour = colour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}
	public LineSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, Colour colour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		Colour = colour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}

	public int ZIndex(int index) => _parent.Colours.GetZIndex(Colour);
}