namespace Phanes.Mapper;

public sealed class TextInstance : Instance<TextSymbol>
{
	public string Text { get; set; }
	
	public HorizontalAlignment HorizontalAlignment { get; set; }
	public VerticalAlignment VerticalAlignment { get; set; }

	public TextInstance(int layer, TextSymbol symbol, string text, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) 
		: base(layer, symbol)
	{
		Text = text;
		HorizontalAlignment = horizontalAlignment;
		VerticalAlignment = verticalAlignment;
	}
	
	public TextInstance(Guid id, int layer, TextSymbol symbol, string text, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
		: base(id, layer, symbol)
	{
		Text = text;
		HorizontalAlignment = horizontalAlignment;
		VerticalAlignment = verticalAlignment;
	}
}

public enum HorizontalAlignment { Left, Center, Right }
public enum VerticalAlignment { Top, Center, Bottom }