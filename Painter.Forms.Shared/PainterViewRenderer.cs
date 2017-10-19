//
// PainterViewRenderer.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using Painter;
using Painter.Forms;
using Xamarin.Forms;
using System.Threading.Tasks;

#if __IOS__
using Xamarin.Forms.Platform.iOS;
using Painter.Forms.iOS;
using NativePainterView = Painter.iOS.PainterView;
#elif __ANDROID__
using Xamarin.Forms.Platform.Android;
using Painter.Forms.Droid;
using NativePainterView = Painter.Android.PainterView;
#endif


[assembly: ExportRenderer(typeof(PainterView), typeof(PainterViewRenderer))]

#if __IOS__
namespace Painter.Forms.iOS
#elif __ANDROID__
namespace Painter.Forms.Droid
#endif
{
	public static class PainterRenderer
	{
		public static void Init() { }
	}

	public class PainterViewRenderer : ViewRenderer<PainterView, NativePainterView>
	{
		private Abstractions.Color _strokeColor;
		public Abstractions.Color StrokeColor
		{
			get
			{
				return _strokeColor;
			}
			set
			{
				_strokeColor = value;
				if (Control != null)
					(Control as NativePainterView).StrokeColor = _strokeColor;
			}
		}

		private EventHandler _finishedStrokeEvent;
		public EventHandler FinishedStrokeEvent
		{
			get
			{
				return _finishedStrokeEvent;
			}
			set
			{
				_finishedStrokeEvent = value;
				if (Control != null)
					(Control as NativePainterView).FinishedStrokeEvent = _finishedStrokeEvent;
			}
		}


		private int _strokeThickness;
		public int StrokeThickness
		{
			get
			{
				return _strokeThickness;
			}
			set
			{
				_strokeThickness = value;
				if (Control != null)
					(Control as NativePainterView).StrokeThickness = _strokeThickness;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<PainterView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				// Instantiate the native control and assign it to the Control property
#if __ANDROID__
				var native = new NativePainterView(Xamarin.Forms.Forms.Context);
#elif __IOS__
				var native = new NativePainterView(new CoreGraphics.CGRect(0, 0, e.NewElement.Bounds.Width, e.NewElement.Bounds.Height));
				native.Opaque = false;
#endif

                //Set the default values that are known
                native.StrokeColor = e.NewElement.StrokeColor;
				native.StrokeThickness = e.NewElement.StrokeThickness;
				native.FinishedStrokeEvent = e.NewElement.FinishedStrokeEvent;

				SetNativeControl(native);
			}

			if (e.OldElement != null)
			{
				e.OldElement.GetJsonEvent -= HandleGetJson;
				e.OldElement.ClearEvent -= ClearImage;
				e.OldElement.LoadJsonEvent -= LoadJson;
				e.OldElement.SetImagePathEvent -= SetImagePathEvent;
				e.OldElement.GetStrokesEvent -= GetStrokesEvent;
			}
			if (e.NewElement != null)
			{
				e.NewElement.GetJsonEvent += HandleGetJson;
				e.NewElement.ClearEvent += ClearImage;
				e.NewElement.LoadJsonEvent += LoadJson;
				e.NewElement.SetImagePathEvent += SetImagePathEvent;
				e.NewElement.GetStrokesEvent += GetStrokesEvent;

				e.NewElement.PropertyChanged += Native_PropertyChanged;
			}

			e.NewElement.RendererInitialized();
		}

		private void Native_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//TODO find a way to have the bindings do this
			StrokeColor = Element.StrokeColor;
			StrokeThickness = Element.StrokeThickness;
		}

		private void HandleGetJson(object sender, PainterView.SaveJsonImageHandler e)
		{
			e.Json = Task.Run(() => Control.GetJson());
		}

		private void ClearImage(object sender, EventArgs e)
		{
			Control.Clear();
		}

		private void LoadJson(object sender, PainterView.LoadJsonEventHandler e)
		{
			Control.LoadJson(e.Json);
		}

		private void SetImagePathEvent(object sender, PainterView.SetImageHandler e)
		{
			Control.LoadImage(e.Path, e.InResources, e.Scaling);
		}

		private void GetStrokesEvent(object sender, PainterView.StrokesHandler e)
		{
			e.GetStrokes = Task.Run(() => Control.Strokes);
		}
	}
}
