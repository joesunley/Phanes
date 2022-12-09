namespace Phanes.Mapper;

public sealed class PointSymbol : Symbol
{
	public List<MapObject> MapObjects { get; set; }
	
	public bool IsLockedToNorth { get; set; }

	public PointSymbol(Map parent, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, List<MapObject> mapObjects, bool isLockedToNorth) 
		: base(parent, name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = mapObjects;
		IsLockedToNorth = isLockedToNorth;
	}
	
	public PointSymbol(Map parent, Guid id, string name, string description, SymbolNumber number, bool isUncrossable, bool isHelperSymbol, List<MapObject> mapObjects, bool isLockedToNorth) 
		: base(parent, id, name, description, number, isUncrossable, isHelperSymbol)
	{
		MapObjects = mapObjects;
		IsLockedToNorth = isLockedToNorth;
	}
}