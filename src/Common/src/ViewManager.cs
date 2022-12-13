using Phanes.Common.Renderer;

namespace Phanes.Common;

public static class ViewManager
{
	private static bool s_hasSet;

	private static readonly Dictionary<Guid, IEnumerable<IShape>> s_objects;

	private static WindowsElement? s_canvas, s_mainWindow;

	static ViewManager()
	{
		s_hasSet = false;

		s_objects = new();
	}

	internal static bool Add(Guid id, IEnumerable<IShape> objects)
	{
		throw new NotImplementedException();
	}

	internal static bool Update(Guid id, IEnumerable<IShape> objects)
	{
		throw new NotImplementedException();
	}

	internal static bool AddOrUpdate(Guid id, IEnumerable<IShape> objects)
	{
		if (!s_hasSet) return false;

		return s_objects.ContainsKey(id) 
			? Update(id, objects) : Add(id, objects);
	}
	
	internal static bool Remove(Guid id)
	{
		throw new NotImplementedException();
	}
	
	internal static bool Clear()
	{
		throw new NotImplementedException();
	}

	public static IEnumerable<IShape> RenderAll()
		=> s_objects.Values.SelectMany(x => x);

	
	public static float Zoom { get; internal set; }

	internal static vec2 LocalMousePos => _Input.GetMousePostion(s_canvas);

	public static bool IsWithinBounds()
		=> _Input.IsWithinBounds(s_canvas, _Input.GetMousePostion());

	public static vec2 TopLeft => s_canvas!.LocalPosition;
	public static vec2 BottomRight => s_canvas!.LocalEndPosition;
	public static vec2 Size => BottomRight - TopLeft;
	public static vec2 Centre => TopLeft + Size / 2;

	internal static void DoZoom(bool isZoomIn)
	{
		if (isZoomIn)
			Zoom += MapperSettings.View_ZoomDelta(Zoom);
		else
			Zoom -= MapperSettings.View_ZoomDelta(Zoom);
		
		if (Zoom < MapperSettings.View_MinZoom)
			Zoom = MapperSettings.View_MinZoom;
	}
	
	public static void ZoomIn() => DoZoom(true);
	public static void ZoomOut() => DoZoom(false);
}

public enum RenderType { Add, Update, Remove, Clear }