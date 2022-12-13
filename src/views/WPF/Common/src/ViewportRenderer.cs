using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Phanes.View.Common;

public static class ViewportRenderer
{
	private static Canvas s_canvas;
	private static Dictionary<Guid, IEnumerable<UIElement>> s_objects;

	static ViewportRenderer()
	{
		s_canvas = new();
		s_objects = new();
	}

	public static void Setup(Canvas canvas)
	{
		s_canvas = canvas;
		s_objects = new();
	}
}