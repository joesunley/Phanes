namespace Phanes.Mapper;

public sealed class AreaSymbol : Symbol, IPathSymbol
{
	public IFill Fill { get; set; }
	
	public Colour BorderColour { get; set; }

	public float Width { get; set; }

	public DashStyle DashStyle { get; set; }
	
	public MidStyle MidStyle { get; set; }

	public AreaSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IFill fill, Colour borderColour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		Fill = fill;
		BorderColour = borderColour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}
	
	public AreaSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IFill fill, Colour borderColour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		Fill = fill;
		BorderColour = borderColour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}
	
	public int ZIndex(int index)
	{
		return index switch
		{
			0 => _parent.Colours.GetZIndex(BorderColour),
			1 => _parent.Colours.GetZIndex((SolidFill)Fill),
			_ => -1,
		};
	}
}
