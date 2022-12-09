namespace Phanes.Mapper;

public sealed class PointSymbol : Symbol
{
	public List<MapObject> MapObjects { get; set; }
	
	public bool IsLockedToNorth { get; set; }

	public PointSymbol(string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, List<MapObject> mapObjects, bool isLockedToNorth) 
		: base(name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = mapObjects;
		IsLockedToNorth = isLockedToNorth;
	}
	
	public PointSymbol(Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, List<MapObject> mapObjects, bool isLockedToNorth) 
		: base(id, name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = mapObjects;
		IsLockedToNorth = isLockedToNorth;
	}
}