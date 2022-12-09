using System.Collections;
using System.Data;
using Phanes.Mapper;

namespace Phanes.Common.Renderer;

public static class RenderExtension
{
	public static IEnumerable<(Symbol, IEnumerable<IShape>)> Render(this Map map)
		=> _Render.Map(map);
	
	public static IEnumerable<IShape> Render(this Symbol sym)
		=> _Render.Symbol(sym);
	
	public static IEnumerable<IShape> Render(this Instance inst)
		=> _Render.Instance(inst);
	
	public static IEnumerable<IShape> Render(this MapObject obj)
		=> _Render.MapObject(obj);
	
	public static IEnumerable<IShape> LiveRender(this IPathSymbol pathSymbol, IEnumerable<vec2> pts)
		=> _Render.LiveSymbol(pathSymbol, pts);
	
	public static IEnumerable<IShape> LiveRender(this IEnumerable<MapObject> objs, vec2 centre)
		=> _Render.LiveMapObjects(objs, centre);
}

file static class _Render
{
	#region Map Objects

	public static IEnumerable<IShape> MapObject(IEnumerable<MapObject> objs)
	{
		List<IShape> shapes = new();
		
		foreach (var obj in objs)
			shapes.AddRange(MapObject(obj));

		return shapes;
	}

	public static IEnumerable<IShape> MapObject(MapObject obj)
	{
		return obj switch
		{
			PointObject point => PointObject(point),
			LineObject line   => LineObject(line),
			AreaObject area   => AreaObject(area),
			TextObject text   => TextObject(text),
			_ => throw new ArgumentOutOfRangeException(nameof(obj)),
		};
	}

	public static IEnumerable<IShape> PointObject(PointObject obj)
	{
		float sum = obj.InnerRadius + obj.OuterRadius;
		float diameter = 2 * sum;

		Ellipse innerEllipse = new()
		{
			Size = new(
				2 * obj.InnerRadius,
				2 * obj.InnerRadius),

			Fill = obj.InnerColour,

			BorderWidth = 0,

			ZIndex = obj.ZIndex(0),
		};

		Ellipse outerEllipse = new()
		{
			Size = new(diameter, diameter),

			BorderWidth = obj.OuterRadius,
			BorderColour = obj.OuterColour,

			ZIndex = obj.ZIndex(1),
		};

		return new IShape[] { innerEllipse, outerEllipse };
	}

	public static IEnumerable<IShape> LineObject(LineObject obj)
	{
		Line line = new()
		{
			Points = obj.Points,

			Colour = obj.Colour,
			Width = obj.Width,

			ZIndex = obj.ZIndex(-1), // input is ignored
		};

		return new IShape[] { line };
	}

	public static IEnumerable<IShape> AreaObject(AreaObject obj)
	{
		Line border = new()
		{
			Points = obj.Points,

			Colour = obj.BorderColour,
			Width = obj.Width,

			ZIndex = obj.ZIndex(0),
		};
        
		if (obj.Fill is SolidFill solidFill)
		{
			Area area = new()
			{
				Points = obj.Points,

				Fill = solidFill.Colour,

				ZIndex = obj.ZIndex(1),
			};

			return new IShape[] { border, area };
		}
		else if (obj.Fill is ObjectFill objFill)
		{
			var fillObjs = ObjectFill(objFill);

			return new IShape[] { border }.Concat(fillObjs);
		}
		else throw new ArgumentException();
	}

	public static IEnumerable<IShape> TextObject(TextObject obj)
	{
		throw new NotImplementedException();
	}

	public static IEnumerable<IShape> ObjectFill(ObjectFill objFill)
	{
		throw new NotImplementedException();
	}
	
	#endregion
	
	#region Symbols

	public static IEnumerable<IShape> Symbol(IEnumerable<Symbol> syms)
	{
		List<IShape> shapes = new();
		
		foreach (Symbol sym in syms)
			shapes.AddRange(Symbol(sym));
		
		return shapes;
	}

	public static IEnumerable<IShape> Symbol(Symbol sym)
	{
		return sym switch
		{
			PointSymbol point => PointSymbol(point),
			LineSymbol line   => LineSymbol(line),
			AreaSymbol area   => AreaSymbol(area),
			TextSymbol text   => TextSymbol(text),
			_ => throw new ArgumentOutOfRangeException(nameof(sym)),
		};
	}

	public static IEnumerable<IShape> PointSymbol(PointSymbol s)
	{
		return MapObject(s.MapObjects);
	}
	
	public static IEnumerable<IShape> LineSymbol(LineSymbol s)
	{
		Phanes.Common.Renderer.Path path = new()
		{
			BorderWidth = s.Width,
			BorderColour = s.Colour,

			IsClosed = false,

			ZIndex = s.ZIndex(0),
		};

		return new IShape[] { path };
	}

	public static IEnumerable<IShape> AreaSymbol(AreaSymbol s)
	{
		Phanes.Common.Renderer.Path border = new()
		{
			BorderWidth = s.Width,
			BorderColour = s.BorderColour,

			ZIndex = s.ZIndex(0),
		};

		switch (s.Fill)
		{
			case SolidFill solidFill: {
				Phanes.Common.Renderer.Path area = new()
				{
					Fill = solidFill.Colour,

					ZIndex = s.ZIndex(1),
				};

				return new IShape[] { border, area };
			}
			case ObjectFill objFill: {
				IEnumerable<IShape> fillObjs = ObjectFill(objFill);

				return new IShape[] { border }.Concat(fillObjs);
			}
			default:
				throw new ArgumentException();
		}
	}
	
	public static IEnumerable<IShape> TextSymbol(TextSymbol s)
	{
		throw new NotImplementedException();
	}

	#endregion
	
	#region Instances
	
	public static IEnumerable<IShape> Instance(IEnumerable<Instance> insts)
	{
		List<IShape> shapes = new();
		
		foreach (Instance inst in insts)
			shapes.AddRange(Instance(inst));
		
		return shapes;
	}
	
	public static IEnumerable<IShape> Instance(Instance inst)
	{
		return inst switch
		{
			PointInstance point => PointInstance(point),
			LineInstance line   => PathInstance(line),
			AreaInstance area   => PathInstance(area),
			TextInstance text   => TextInstance(text),
			_ => throw new ArgumentOutOfRangeException(nameof(inst)),
		};
	}
	
	public static IEnumerable<IShape> PointInstance(PointInstance inst)
	{
		foreach (IShape el in PointSymbol(inst.Symbol))
		{
			if (el is Ellipse ellipse)
			{
				vec2 transform = new(
					inst.Centre.X - ellipse.Size.X / 2,
					inst.Centre.Y - ellipse.Size.Y / 2
					);

				el.TopLeft = transform;
			}
			else
				el.TopLeft = inst.Centre;

			yield return el;
		}
	}

	public static IEnumerable<IShape> PathInstance(PathInstance inst)
	{
		List<Renderer.IPathSegment> segments = new();

		foreach (var seg in inst.Segments)
		{
			switch (seg)
			{
				case LinearPath linear:
					segments.Add(new PolyLineSegment() { StartPoint = linear.First(), Points = linear.Skip(1).ToList() });
					break;
				case BezierPath bezier:
					segments.Add(_Utils.CreateBezierSegment(bezier));
					break;
				default:
					throw new ArgumentException(nameof(inst.Segments));
			}
		}

		List<IShape> renders = new();

		foreach (IShape el in Symbol((Symbol)inst.Symbol))
		{
			if (el is Renderer.Path p)
			{
				p.Segments = segments;

				if (inst.Symbol.DashStyle.HasDash)
					p.DashArray = _Utils.CreateDashArray(_Utils.CalculateLengthOfPath(inst.Segments), inst.Symbol.DashStyle, inst.Symbol.Width);

				renders.Add(p);
			}
			else throw new ArgumentException(nameof(p));
		}

		if (inst.Symbol.MidStyle.HasMid)
		{
			IEnumerable<vec2> mids = _Utils.CalculateMidPoints(inst.Segments, inst.Symbol.MidStyle);

			foreach (vec2 p in mids)
			{
				// Add MidPoints -> Live Render MapObjects
			}
		}

		return renders;
	}

	public static IEnumerable<IShape> TextInstance(TextInstance inst)
	{
		throw new NotImplementedException();
	}

	#endregion
	
	#region Live

	public static IEnumerable<IShape> LiveSymbol(IPathSymbol sym, IEnumerable<vec2> points)
	{
		return sym switch
		{
			LineSymbol line => LiveLineSymbol(line, points),
			AreaSymbol area => LiveAreaSymbol(area, points),
			_ => throw new ArgumentOutOfRangeException(nameof(sym)),
		};
	}

	public static IEnumerable<IShape> LiveLineSymbol(LineSymbol sym, IEnumerable<vec2> points)
	{
		List<vec2> pts = points.ToList();

		Line line = new()
		{
			Colour = sym.Colour,
			Width = sym.Width,
			
			Points = pts,
			
			ZIndex = sym.ZIndex(-1),
		};
		
		if (sym.DashStyle.HasDash)
			line.DashArray = _Utils.CreateDashArray(pts.Length(), sym.DashStyle, sym.Width);

		List<IShape> renders = new();

		if (sym.MidStyle.HasMid)
		{
			IEnumerable<vec2> mids = _Utils.CalculateMidPoints(pts, sym.MidStyle);

			foreach (vec2 p in mids)
			{
				// Add MidPoints
			}
		}
		
		renders.Add(line);

		return renders;
	}

	public static IEnumerable<IShape> LiveAreaSymbol(AreaSymbol sym, IEnumerable<vec2> points)
	{
		List<vec2> pts = points.ToList();

		Line border = new()
		{
			Colour = sym.BorderColour,
			Width = sym.Width,

			Points = pts,

			ZIndex = sym.ZIndex(0),
		};

		switch (sym.Fill)
		{
			case SolidFill solidFill: {
				Area area = new()
				{
					Fill = solidFill.Colour,
				
					Points = pts,
				
					ZIndex = sym.ZIndex(1),
				};

				return new IShape[] { border, area };
			}
			case ObjectFill objFill:
				return new IShape[] { border }.Concat(ObjectFill(objFill));
			default: 
				throw new NotImplementedException();
		}
	}

	public static IEnumerable<IShape> LiveMapObjects(IEnumerable<MapObject> objs, vec2 centre)
	{
		foreach (MapObject obj in objs)
		{
			foreach (IShape el in MapObject(obj))
			{
				if (el is Ellipse ellipse)
				{
					vec2 transform = new(
						centre.X - ellipse.Size.X / 2,
						centre.Y - ellipse.Size.Y / 2
						);

					el.TopLeft = transform;
				}
				else
					el.TopLeft = centre;

				yield return el;
			}

		}
	}

	#endregion

	public static IEnumerable<(Symbol, IEnumerable<IShape>)> Map(Map map)
	{
		List<(Symbol, IEnumerable<IShape>)> objs = new();

		foreach (Instance inst in map.Instances)
			objs.Add((inst.Symbol, Instance(inst)));

		return objs;
	}
}

file static class _Utils
{
	public static PolyBezierSegment CreateBezierSegment(BezierPath path)
	{
		throw new NotImplementedException();
	}
	
	// ReSharper disable once ReturnTypeCanBeEnumerable.Local
	public static double[] CreateDashArray(float lineLength, DashStyle dashStyle, float lineWidth)
	{
		if (dashStyle.GroupSize > 1)
		{
			float minLen = ((dashStyle.DashLength + dashStyle.GapLength) * dashStyle.GroupSize) - dashStyle.GapLength + dashStyle.GroupGapLength;

			if (lineLength < minLen)
				return Array.Empty<double>();

			lineLength += dashStyle.GroupGapLength;

			float maxSingleDash = (((dashStyle.DashLength * 1.2f) + dashStyle.GapLength) * dashStyle.GroupSize) - dashStyle.GapLength + dashStyle.GroupGapLength;
			float minTimes = lineLength / maxSingleDash;

			float dashCount = MathF.Round(minTimes);
			float combinedGroupSize = lineLength / dashCount;

			float combinedDashSize = combinedGroupSize - dashStyle.GroupGapLength;
			float individualDashSize = (combinedDashSize + dashStyle.GapLength) / dashStyle.GroupSize;

			float dashSize = individualDashSize - dashStyle.GapLength;

			List<double> array = new();

			for (int i = 0; i < dashStyle.GroupSize - 1; i++)
			{
				array.Add(dashSize / lineWidth);
				array.Add(dashStyle.GapLength / lineWidth);
			}
			array.Add(dashSize / lineWidth);
			array.Add(dashStyle.GroupGapLength / lineWidth);

			return array.ToArray();
		}
		else
		{
			float minLen = dashStyle.GapLength + (1.6f * dashStyle.DashLength);
			if (lineLength < minLen)
				return Array.Empty<double>(); // No Dash

			lineLength += dashStyle.GapLength;

			float maxSingleDash = dashStyle.GapLength + (1.2f * dashStyle.DashLength);
			float minTimes = lineLength / maxSingleDash;

			float dashCount = MathF.Round(minTimes); // Int
			float combinedDashSize = lineLength / dashCount;

			float dashSize = combinedDashSize - dashStyle.GapLength;

			return new double[] { dashSize / lineWidth, dashStyle.GapLength / lineWidth };
		}
	}

	public static float CalculateLengthOfPath(PathCollection pC)
	{
		float totalLen = 0f;
		
		foreach (Phanes.Mapper.IPathSegment seg in pC)
		{
			switch (seg)
			{
				case LinearPath line:
					totalLen += line.Length();
					break;
				case BezierPath bezier:
					totalLen += InterpolateAlongBezierPath(bezier, 0.05f).Length();
					break;
			}
		}
		
		return totalLen;
	}

	public static IEnumerable<vec2> InterpolateAlongBezierPath(BezierPath bezPath, float resolution)
	{
		List<vec2> points = new();

		for (int i = 1; i < bezPath.Count(); i++)
		{
			BezierPoint early = bezPath[i - 1];
			BezierPoint late = bezPath[i];

			for (float t = 0f; t <= 1; t += resolution)
				points.Add(CalculateBezierLerp(early, late, t));
		}

		return points;
	}

	public static vec2 CalculateBezierLerp(BezierPoint a, BezierPoint b, float t)
	{
		vec2 p0 = vec2.Lerp(a.Anchor, a.LateControl, t);
		vec2 p1 = vec2.Lerp(a.LateControl, b.EarlyControl, t);
		vec2 p2 = vec2.Lerp(b.EarlyControl, b.Anchor, t);

		vec2 d = vec2.Lerp(p0, p1, t);
		vec2 e = vec2.Lerp(p1, p2, t);

		return vec2.Lerp(d, e, t);
	}

	public static IEnumerable<vec2> CalculateMidPoints(PathCollection pC, MidStyle midStyle)
	{
		throw new NotImplementedException();
	}

	public static IEnumerable<vec2> CalculateMidPoints(IEnumerable<vec2> points, MidStyle midStyle)
	{
		throw new NotImplementedException();
	}
}
