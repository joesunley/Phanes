using Phanes.Mapper;

namespace Phanes.Common;

internal static class Settings
{
	public static double TickSpeed => 5;

	public static float Grid_Thickness => 0.1f;
}

internal static class MapperSettings
{
	public static float Draw_Opacity => 0.5f;

	#region Viewport

	public static float View_ZoomDelta(float currentZoom) => 0.1f * currentZoom;
	public static float View_MinZoom => 0.01f;

	#endregion

	#region Select Handles 

	// public static int Handles_ZIndex => Manager.ActiveMap().Map.Colours.Count() + 11;
	public static Colour Handles_AnchorColour => 0x000000;
	public static Colour Handles_ControlColour => 0xffa600;
	public static float Handles_Radius => (float)((1 / ZoomFunc(ViewManager.Zoom)));
	public static float Handles_Width => Handles_Radius * .3f;

	#endregion

	#region Select BoundingBox

	public static float BoundingBox_Offset => 2f * (1 / (float)ZoomFunc(ViewManager.Zoom));
	public static float BoundingBox_Width => 0.5f * (1 / (float)ZoomFunc(ViewManager.Zoom));
	public static vec4 BoundingBox_MinimumSize(vec2 centre)
	{
		float fact = 1 / (float)ZoomFunc(ViewManager.Zoom);

		const float constant = 2f;

		vec2 topLeft = new(fact * -constant, fact * -constant);
		vec2 bottomRight = new(fact * constant, fact * constant);

		topLeft += centre;
		bottomRight += centre;

		return new(topLeft, bottomRight);
	}
	public static double[] BoundingBox_DashArray => new double[] { 6, 1 };
	public static Colour BoundingBox_Colour => 0xffa600;
	// public static int BoundingBox_ZIndex => Manager.ActiveMap().Map.Colours.Count() + 12;

	#endregion

	#region Selection

	public static float Selection_ObjectTolerance => 2f;
	public static float Selection_PointTolerance => 1f;

	#endregion
}