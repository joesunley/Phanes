namespace Phanes.Mapper;

public sealed class TextInstance : Instance<TextSymbol>
{
	public TextInstance(int layer, TextSymbol symbol) : base(layer, symbol)
	{
		throw new NotImplementedException();
	}

	public TextInstance(Guid id, int layer, TextSymbol symbol) : base(id, layer, symbol)
	{
		throw new NotImplementedException();
	}
}