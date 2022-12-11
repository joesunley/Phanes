namespace Phanes.Mapper;

public sealed class PointSymbol : Symbol
{
	public List<MapObject> MapObjects { get; set; }
	
	public bool IsRotatable { get; set; }

	public PointSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IEnumerable<MapObject> mapObjects, bool isRotatable) 
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = new(mapObjects);
		IsRotatable = isRotatable;
	}
	
	public PointSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, IEnumerable<MapObject> mapObjects, bool isRotatable) 
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = new(mapObjects);
		IsRotatable = isRotatable;
	}
}