using System.Windows;
using Phanes.Common;

internal static class _Utils
{
	public static float GetScaling()
	{
		float output = 1f;
		
		Application.Current.Dispatcher.Invoke(() =>
		{
			try
			{
				double scaling = System.Windows.PresentationSource.FromVisual(Application.Current.MainWindow)
									   .CompositionTarget
									   .TransformToDevice.M11;

				output = (float)scaling;
			}
			catch
			{
				// Log error
			}
		});

		return output;
	}

	public static vec2 Position(this UIElement el, UIElement? parent = null)
	{
		vec2 output = vec2.Zero;

		Application.Current.Dispatcher.Invoke(() =>
		{
			try
			{
				vec2 gPoint = el.PointToScreen(new(0, 0)).ToVec2();

				switch (parent)
				{
					case null:
						output = gPoint;
						break;
					case Window win: {
						vec2 parentPoint = (win.Left + 7, win.Top);
						output = gPoint - parentPoint;
					}
						break;
					default: {
						vec2 parentPoint = parent.PointToScreen(new(0, 0)).ToVec2();
						output = gPoint - parentPoint;
					}
						break;
				}
			}
			catch
			{
				// Log error
			}
		});

		return output;
	}
	
	public static vec2 ToVec2(this Point p)
		=> new(p.X, p.Y);
	public static Point ToPoint(this vec2 v2)
		=> new(v2.X, v2.Y);
}