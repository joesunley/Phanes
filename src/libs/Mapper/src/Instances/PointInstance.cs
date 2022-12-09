namespace Phanes.Mapper;

public sealed class PointInstance : Instance<PointSymbol>
{
	public vec2 Centre { get; set; }

	public PointInstance(int layer, PointSymbol symbol, vec2 centre)
		: base(layer, symbol)
	{
		Centre = centre;
	}

	public PointInstance(Guid id, int layer, PointSymbol symbol, vec2 centre)
		: base(id, layer, symbol)
	{
		Centre = centre;
	}

	public override vec4 GetBoundingBox()
	{
		vec2 topLeft = vec2.MaxValue,
			 bottomRight = vec2.MinValue;

		foreach (MapObject mapObject in Symbol.MapObjects)
		{
			vec4 bBox = mapObject.GetBoundingBox() + Centre;

			topLeft = vec2.Min(topLeft, (vec2)bBox);
			bottomRight = vec2.Max(bottomRight, (bBox.Z, bBox.W));
		}

		return new(topLeft, bottomRight);
	}
}