namespace Phanes.Mapper;

public sealed class AreaSymbol : Symbol, IPathSymbol
{
	public IFill Fill { get; set; }
	
	public Colour BorderColour { get; set; }

	public float Width { get; set; }
	
	public DashStyle DashStyle { get; set; }
	
	public MidStyle MidStyle { get; set; }

	public AreaSymbol(string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IFill fill, Colour borderColour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(name, description, number, isUncrossable, isHelperSymbol)
	{
		Fill = fill;
		BorderColour = borderColour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}
	
	public AreaSymbol(Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IFill fill, Colour borderColour, float width, DashStyle dashStyle, MidStyle midStyle) 
		: base(id, name, description, number, isUncrossable, isHelperSymbol)
	{
		Fill = fill;
		BorderColour = borderColour;
		Width = width;
		DashStyle = dashStyle;
		MidStyle = midStyle;
	}
}