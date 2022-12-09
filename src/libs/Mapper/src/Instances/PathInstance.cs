using System.Collections;

namespace Phanes.Mapper;

public abstract class PathInstance<T> : Instance<T>, PathInstance where T : Symbol
{
	public PathCollection Segments { get; set; }
    
	IPathSymbol PathInstance.Symbol
	{
		get => (IPathSymbol)this.Symbol;
		set => Symbol = (T)value;
	}

	protected PathInstance(int layer, T symbol, PathCollection segments)
		: base(layer, symbol)
	{
		Segments = segments;
	}

	protected PathInstance(Guid id, int layer, T symbol, PathCollection segments)
		: base(id, layer, symbol)
	{
		Segments = segments;
	}

	public override vec4 GetBoundingBox()
	{
		vec2 topLeft = vec2.MaxValue,
			 bottomRight = vec2.MinValue;

		foreach (IPathSegment segment in Segments)
		{
			if (segment is LinearPath line)
			{
				foreach (vec2 point in line.GetAllPoints())
				{
					topLeft = vec2.Min(topLeft, point);
					bottomRight = vec2.Max(bottomRight, point);
				}
			}
			else if (segment is BezierPath bezier)
			{
				for (int i = 1; i < bezier.Count(); i++)
				{
					BezierPoint a = bezier[i - 1], b = bezier[i];

					// TODO: Optimise this
					for (float t = 0; t <= 1; t += 0.01f)
					{
						vec2 res = BezierLerp(a, b, t);

						topLeft = vec2.Min(topLeft, res);
						bottomRight = vec2.Max(bottomRight, res);
					}
				}
			}
		}

		return new(topLeft, bottomRight);
	}

	private static vec2 BezierLerp(BezierPoint a, BezierPoint b, float t)
	{
		vec2 p0 = vec2.Lerp(a.Anchor, a.LateControl, t);
		vec2 p1 = vec2.Lerp(a.LateControl, b.EarlyControl, t);
		vec2 p2 = vec2.Lerp(b.EarlyControl, b.Anchor, t);

		vec2 d = vec2.Lerp(p0, p1, t);
		vec2 e = vec2.Lerp(p1, p2, t);

		return vec2.Lerp(d, e, t);
	}
}

public sealed class LineInstance : PathInstance<LineSymbol>
{
	public LineInstance(int layer, LineSymbol symbol, PathCollection segments)
		: base(layer, symbol, segments) { }

	public LineInstance(Guid id, int layer, LineSymbol symbol, PathCollection segments)
		: base(id, layer, symbol, segments) { }
}

public sealed class AreaInstance : PathInstance<AreaSymbol>
{
	public AreaInstance(int layer, AreaSymbol symbol, PathCollection segments)
		: base(layer, symbol, segments) { }

	public AreaInstance(Guid id, int layer, AreaSymbol symbol, PathCollection segments)
		: base(id, layer, symbol, segments) { }
}

public interface PathInstance : Instance
{
	PathCollection Segments { get; }
	
	new IPathSymbol Symbol { get; set; }
}

public sealed class PathCollection : List<IPathSegment>
{
	public IEnumerable<vec2> GetAllPoints()
	{
		List<vec2> points = new();

		foreach (IPathSegment obj in this) 
			points.AddRange(obj.GetAllPoints());

		return points;
	}

	public IEnumerable<vec2> GetAnchorPoints()
	{
		List<vec2> points = new();

		foreach (IPathSegment obj in this) 
			points.AddRange(obj.GetAnchorPoints());

		return points;
	}

	public IEnumerable<vec2> GetControlPoints()
	{
		List<vec2> points = new();

		foreach (IPathSegment obj in this)
		{
			if (obj is BezierPath bez)
			{
				points.AddRange(bez.Select(x => x.EarlyControl));
				points.AddRange(bez.Select(x => x.LateControl));
			}
		}

		return points;
	}
}

public interface IPathSegment
{
	IEnumerable<vec2> GetAllPoints();
	IEnumerable<vec2> GetAnchorPoints();
	IEnumerable<vec2> GetControlPoints();
}

public struct BezierPoint
{
	public vec2 Anchor { get; set; }
	public vec2 EarlyControl { get; set; }
	public vec2 LateControl { get; set; }

	public BezierPoint(vec2 anchor, vec2 earlyControl, vec2 lateControl)
	{
		Anchor = anchor;
		EarlyControl = earlyControl;
		LateControl = lateControl;
	}
}

public readonly struct BezierPath : IPathSegment, IEnumerable<BezierPoint>
{
	private readonly List<BezierPoint> _points;

	public BezierPoint this[int index]
	{
		get => _points[index];
		set => _points[index] = value;
	}


	public BezierPath(IList<BezierPoint>? points = null)
	{
		points ??= Array.Empty<BezierPoint>();
		_points = new(points);
	}

	public IEnumerable<vec2> GetAllPoints() 
		=> _points.SelectMany(point => new[] { point.EarlyControl, point.Anchor, point.LateControl }).ToList();

	public IEnumerable<vec2> GetControlPoints() 
		=> _points.SelectMany(point => new[] { point.EarlyControl, point.LateControl }).ToList();

	public IEnumerable<vec2> GetAnchorPoints() 
		=> _points.Select(x => x.Anchor).ToList();

	public IEnumerator<BezierPoint> GetEnumerator() => _points.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();
}

public readonly struct LinearPath : IPathSegment, IEnumerable<vec2>
{
	private readonly List<vec2> _points;

	public vec2 this[int index]
	{
		get => _points[index];
		set => _points[index] = value;
	}

	public LinearPath(IList<vec2>? points = null)
	{
		points ??= new List<vec2>();
		_points = new(points);
	}

	public int IndexOf(vec2 v2) 
		=> _points.IndexOf(v2);

	public IEnumerable<vec2> GetAllPoints() 
		=> _points;

	public IEnumerable<vec2> GetAnchorPoints() 
		=> _points;

	public IEnumerable<vec2> GetControlPoints()
		=> Enumerable.Empty<vec2>();

	public IEnumerator<vec2> GetEnumerator() => _points.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();
}