using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Phanes.Mapper;
using Sys = System.Windows.Shapes;
using Pha = Phanes.Common.Renderer;

namespace Phanes.View.Common;

public static class Render
{
	public static IEnumerable<UIElement> ConvertCollection(IEnumerable<Pha.IShape> shapes)
	{
		foreach (Pha.IShape el in shapes)
		{
			switch (el)
			{
				case Pha.Rectangle r: yield return _Render.Rectangle(r); break;
				case Pha.Ellipse e: yield return _Render.Ellipse(e); break;
				case Pha.Line l: yield return _Render.Line(l); break;
				case Pha.Area a: yield return _Render.Area(a); break;
				case Pha.Path p: yield return _Render.Path(p); break;
				case Pha.Text t: yield return _Render.Text(t); break;
			}
		}
	}
}

file static class _Render
{
	public static Sys.Rectangle Rectangle(Pha.Rectangle rect)
	{
		Sys.Rectangle output = new()
		{
			Opacity = rect.Opacity,

			Width = rect.Size.X,
			Height = rect.Size.Y,

			Fill = _Utils.ColourToBrush(rect.Fill),

			Stroke = _Utils.ColourToBrush(rect.BorderColour),
			StrokeThickness = rect.BorderWidth,

			StrokeDashArray = new(rect.DashArray),
		};

		output.SetTopLeft(rect.TopLeft);
		output.SetZIndex(rect.ZIndex);

		return output;
	}

	public static Sys.Ellipse Ellipse(Pha.Ellipse ellipse)
	{
		Sys.Ellipse output = new()
		{
			Opacity = ellipse.Opacity,

			Width = ellipse.Size.X,
			Height = ellipse.Size.Y,

			Fill = _Utils.ColourToBrush(ellipse.Fill),

			Stroke = _Utils.ColourToBrush(ellipse.BorderColour),
			StrokeThickness = ellipse.BorderWidth,

			StrokeDashArray = new(ellipse.DashArray),
		};
		
		output.SetTopLeft(ellipse.TopLeft);
		output.SetZIndex(ellipse.ZIndex);
		
		return output;
	}

	public static Sys.Polyline Line(Pha.Line line)
	{
		Sys.Polyline output = new()
		{
			Opacity = line.Opacity,

			Points = line.Points.ToPointCollection(),

			Stroke = _Utils.ColourToBrush(line.Colour),
			StrokeThickness = line.Width,

			StrokeDashArray = new(line.DashArray),
		};

		output.SetTopLeft(line.TopLeft);
		output.SetZIndex(line.ZIndex);

		return output;
	}

	public static Sys.Polygon Area(Pha.Area area)
	{
		Sys.Polygon output = new()
		{
			Opacity = area.Opacity,
			
			Points = area.Points.ToPointCollection(),
			
			Fill = _Utils.ColourToBrush(area.Fill),
			
			Stroke = _Utils.ColourToBrush(area.BorderColour),
			StrokeThickness = area.BorderWidth,
			
			StrokeDashArray = new(area.DashArray),
		};
		
		output.SetTopLeft(area.TopLeft);
		output.SetZIndex(area.ZIndex);
		
		return output;
	}

	public static Sys.Path Path(Pha.Path path)
	{
		PathFigureCollection figures = new();

		foreach (Pha.IPathSegment seg in path.Segments)
		{
			figures.Add(seg switch
			{
				Pha.PolyLineSegment l => _Utils.CreateLinearPathFigure(l),
				Pha.PolyBezierSegment b => _Utils.CreateBezierPathFigure(b),
				_ => throw new InvalidOperationException(),
			});
		}

		PathGeometry geom = new(figures);
		
		Sys.Path output = new()
		{
			Opacity = path.Opacity,
			
			Data = geom,
			
			Fill = _Utils.ColourToBrush(path.Fill),
			
			Stroke = _Utils.ColourToBrush(path.BorderColour),
			StrokeThickness = path.BorderWidth,
			
			StrokeDashArray = new(path.DashArray),
		};
		
		output.SetTopLeft(path.TopLeft);
		output.SetZIndex(path.ZIndex);
		
		return output;
	}

	public static TextBlock Text(Pha.Text text)
	{
		throw new NotImplementedException();
		
		// // Convert to text block
		// TextBlock output = new()
		// {
		// 	Opacity = text.Opacity,
		//
		// 	Text = text.Content,
		// 	FontSize = text.Font.Size,
		// 	FontFamily = new(text.Font.FontFamily),
		// 	FontStyle = text.FontStyle,
		// 	FontWeight = FontWeights.Normal,
		// 	Foreground = _Utils.ColourToBrush(text.Colour),
		//
		// 	TextAlignment = text.Alignment,
		// 	TextWrapping = text.Wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
		// };
		//
		// System.Windows.FontStyle
	}
}

file static class _Utils
{
	public static SolidColorBrush ColourToBrush(Colour col)
	{
		var rgba = col.ToRGBA();
		
		Color color = Color.FromArgb(rgba.a, rgba.r, rgba.g, rgba.b);

		return new(color);
	}

	public static void SetTopLeft(this DependencyObject el, vec2 v2)
	{
		el.SetValue(Canvas.LeftProperty, (double)v2.X);
		el.SetValue(Canvas.TopProperty, (double)v2.Y);
	}

	public static void SetZIndex(this DependencyObject el, int i)
	{
		el.SetValue(Panel.ZIndexProperty, i);
	}
	
	public static PointCollection ToPointCollection(this IEnumerable<vec2> v2s)
		=> new(v2s.Select(v2 => new Point(v2.X, v2.Y)));

	public static Point ToPoint(this vec2 v2)
		=> new(v2.X, v2.Y);

	public static PathFigure CreateLinearPathFigure(Pha.PolyLineSegment seg)
	{
		PathFigure fig = new() { StartPoint = seg.StartPoint.ToPoint() };

		PolyLineSegment poly = new(seg.Points.Select(x => x.ToPoint()), false);
		
		fig.Segments.Add(poly);
		
		return fig;
	}

	public static PathFigure CreateBezierPathFigure(Pha.PolyBezierSegment seg)
	{
		throw new NotImplementedException();
	}
}