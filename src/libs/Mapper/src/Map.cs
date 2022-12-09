namespace Phanes.Mapper;

public sealed class Map
{
	public string Title { get; set; }
	
	public ColourStore Colours { get; set; }
	
	public SymbolStore Symbols { get; set; }
	
	public InstanceStore Instances { get; set; }
	
	public Map(string title = "")
	{
		Title = title;
		
		Colours = new ColourStore();
		Symbols = new SymbolStore();
		Instances = new InstanceStore();
	}
	
	public Map(string title, ColourStore colours, SymbolStore symbols, InstanceStore instances)
	{
		Title = title;

		Colours = colours;
		Symbols = symbols;
		Instances = instances;
	}
}