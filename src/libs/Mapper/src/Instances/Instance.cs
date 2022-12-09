namespace Phanes.Mapper;

[DebuggerDisplay("{Symbol.Name}, {Id}")]
public abstract class Instance<T> : Instance where T : Symbol
{
	public Guid Id { get; init; }
	public int Layer { get; set; }
	public float Opacity { get; set; }
	public T Symbol { get; set; }
	Symbol Instance.Symbol
	{
		get => Symbol;
		set => Symbol = (T)value;
	}

	protected Instance(int layer, T symbol)
	{
		Id = Guid.NewGuid();
		Layer = layer;

		Symbol = symbol;
	}

	protected Instance(Guid id, int layer, T symbol)
	{
		Id = id;
		Layer = layer;

		Symbol = symbol;
	}

	public virtual int ZIndex(int index) => -1;

	public virtual vec4 GetBoundingBox() => vec4.Zero;
}

public interface Instance : IStorable
{
	public int Layer { get; set; }
	public float Opacity { get; set; }
	public Symbol Symbol { get; set; }

	public vec4 GetBoundingBox();
}