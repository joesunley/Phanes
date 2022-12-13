using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Phanes.Mapper;
using Phanes.Common;
using Phanes.Common.Renderer;

namespace Phanes.View.Common;

public partial class MapViewer : UserControl
{
	private Map _map;

	private string _filter;
	public readonly WindowsElement WindowsElement;
	private bool _suspendDrag;

	private vec2? _lastDragPoint = null;
	
	public MapViewer()
	{
		InitializeComponent();

		WindowsElement = new();

		Input.MouseDrag += args =>
		{
			OnUpdate();

			vec2 mousePos = Input.MousePosition();

			_lastDragPoint ??= mousePos;

			if (args.MouseButton == Phanes.Common.MouseButton.Middle && Input.IsWithinBounds(WindowsElement))
			{
				if (_suspendDrag) return;

				vec2 delta = mousePos - _lastDragPoint!.Value;

				_lastDragPoint = mousePos;

				Application.Current.Dispatcher.Invoke(() =>
				{
					Cursor = Cursors.SizeAll;

					scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - delta.X);
					scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - delta.Y);
				});
			}
		};

		Input.MouseUp += _ =>
		{
			_lastDragPoint = null;

			Application.Current.Dispatcher.Invoke(() => Cursor = Cursors.Arrow);
		};

		Input.MouseWheel += args =>
		{
			OnUpdate();
			
			if (!Input.IsWithinBounds(WindowsElement)) return;

			if (args.Direction == MouseWheelDirection.Up)
				ViewManager.ZoomIn();
			else
				ViewManager.ZoomOut();

			Dispatcher.Invoke(() =>
			{
				scaleTransform.ScaleX = ViewManager.Zoom;
				scaleTransform.ScaleY = ViewManager.Zoom;
			});
		};

		// prevents the canvas from scrolling with mouse wheel
		scrollViewer.PreviewMouseWheel += (_, e) => e.Handled = true;
	}

	public void Load(Map map)
	{
		_map = map;
		_filter = CreateFullFilter();

		UpdateCanvas();
	}

	public void Load(string filePath)
	{
		_map = Phanes.Common.IO.Mapper.Load(filePath);
		_filter = CreateFullFilter();

		UpdateCanvas();
	}

	private void UpdateCanvas()
	{
		canvas.Children.Clear();

		var render = _map.Render();

		var dict = render.ToDictionary(r => r.Item1.Name, r => r.Item2);

		string[] splitFilter = _filter.Split(';');

		List<IShape> final = new();
		foreach (string item in splitFilter)
		{
			if (dict.ContainsKey(item))
				final.AddRange(dict[item]);
		}

		IEnumerable<UIElement> els = Render.ConvertCollection(final);
		
		foreach (UIElement el in els)
			canvas.Children.Add(el);
	}
	
	public void UpdateFilter(string filter, bool additive)
	{
		if (additive)
			_filter = filter;
		else
		{
			List<string> splitFilter = CreateFullFilter().Split(';').ToList();
			string[] splitInpFilter = filter.Split(';');

			foreach (string item in splitInpFilter)
			{
				if (splitFilter.Contains(item))
					splitFilter.Remove(item);
			}

			_filter = splitFilter.Aggregate(string.Empty, (current, str) => current + $"{str};");
		}

		UpdateCanvas();
	}

	private string CreateFullFilter()
	{
		return _map.Symbols.Aggregate(string.Empty, (current, sym) => current + $"{sym.Name};");
	}

	private void OnUpdate()
	{
		WindowsElement.Scaling = _Utils.GetScaling();

		vec2 gPos = this.Position();
		vec2 gEndPos = gPos + new vec2(ActualHeight, ActualHeight) * WindowsElement.Scaling;

		vec2 lPos = (scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
		vec2 lEndPos = lPos + new vec2(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight) * WindowsElement.Scaling;
		
		WindowsElement.GlobalPosition = gPos;
		WindowsElement.GlobalEndPosition = gEndPos;
		WindowsElement.LocalPosition = lPos / ViewManager.Zoom;
		WindowsElement.LocalEndPosition = lEndPos / ViewManager.Zoom;
	}
}