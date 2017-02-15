using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Windows.UI.Input;
using Painter.Abstractions;
using Windows.UI.Input.Inking.Core;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Painter.UWP
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PainterView : UserControl
	{
		List<Stroke> strokes;
		Stroke currentStroke;

		public PainterView()
		{
			this.InitializeComponent();

			InkDrawingAttributes inkDrawingAttributes = new InkDrawingAttributes();
			inkDrawingAttributes.Color = Windows.UI.Colors.Blue;
			inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(inkDrawingAttributes);

			inkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;

			CoreInkIndependentInputSource core = CoreInkIndependentInputSource.Create(inkCanvas.InkPresenter);
			core.PointerPressing += Core_PointerPressing;
			core.PointerMoving += Core_PointerMoving;
			core.PointerReleasing += Core_PointerReleasing;

			strokes = new List<Stroke>();
			
			DrawLines();
		}

		public string GetJson()
		{
			return JsonConvert.SerializeObject(strokes);
		}

		public async Task Clear()
		{
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				inkCanvas.InkPresenter.StrokeContainer.Clear();
			});
		}

		public void LoadJson(string json)
		{
			try
			{
				strokes = JsonConvert.DeserializeObject<List<Stroke>>(json);
				DrawLines();
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
			}
		}

		private async void DrawLines()
		{
			await Clear();
			foreach(var stroke in strokes)
			{
				InkStrokeBuilder builder = new InkStrokeBuilder();
				List<Windows.Foundation.Point> points = new List<Windows.Foundation.Point>();
				stroke.Points.ForEach(x => points.Add(new Windows.Foundation.Point(x.X, x.Y)));
				InkStroke curStroke = builder.CreateStroke(points);

				InkDrawingAttributes strokeDrawingAttributes = new InkDrawingAttributes();
				strokeDrawingAttributes.Color = Windows.UI.ColorHelper.FromArgb((byte)(stroke.StrokeColor.A * 255.0), (byte)(stroke.StrokeColor.R * 255.0), (byte)(stroke.StrokeColor.G * 255.0), (byte)(stroke.StrokeColor.B * 255.0));
				curStroke.DrawingAttributes = strokeDrawingAttributes;

				inkCanvas.InkPresenter.StrokeContainer.AddStroke(curStroke);
			}
		}

		private void Core_PointerPressing(CoreInkIndependentInputSource sender, Windows.UI.Core.PointerEventArgs e)
		{
			var pos = e.CurrentPoint.Position;

			currentStroke = new Stroke()
			{
				StrokeColor = new Color(0, 0, 1, 1),
				Thickness = 1
			};
			currentStroke.Points.Add(new Abstractions.Point(pos.X, pos.Y));
		}

		private void Core_PointerMoving(CoreInkIndependentInputSource sender, Windows.UI.Core.PointerEventArgs e)
		{
			var pos = e.CurrentPoint.Position;
			
			currentStroke.Points.Add(new Abstractions.Point(pos.X, pos.Y));
		}

		private void Core_PointerReleasing(CoreInkIndependentInputSource sender, Windows.UI.Core.PointerEventArgs e)
		{
			var pos = e.CurrentPoint.Position;

			currentStroke.Points.Add(new Abstractions.Point(pos.X, pos.Y));

			strokes.Add(currentStroke);
		}
	}
}
