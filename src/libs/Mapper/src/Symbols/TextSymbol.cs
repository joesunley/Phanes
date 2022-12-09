namespace Phanes.Mapper;

public sealed class TextSymbol : Symbol
{
	public TextSymbol(string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol)
		: base(name, description, number, isUncrossable, isHelperSymbol)
	{
		throw new NotImplementedException();
	}

	public TextSymbol(Guid id, string name, string description, SymbolNumber number, bool isUncrossable,
					  bool isHelperSymbol)
		: base(id, name, description, number, isUncrossable, isHelperSymbol)
	{
		throw new NotImplementedException();
	}
}