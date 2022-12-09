namespace Phanes.Mapper;

public sealed class TextSymbol : Symbol
{
	public TextSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol)
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		throw new NotImplementedException();
	}

	public TextSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable,
					  bool isHelperSymbol)
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		throw new NotImplementedException();
	}
}