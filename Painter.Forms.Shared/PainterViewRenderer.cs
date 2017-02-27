//
// PainterViewRenderer.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
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
#elif WINDOWS_UWP
using Xamarin.Forms.Platform.UWP;
using Painter.Forms.UWP;
using NativePainterView = Painter.UWP.PainterView;
#endif


[assembly: ExportRenderer(typeof(PainterView), typeof(PainterViewRenderer))]

#if WINDOWS_UWP
namespace Painter.Forms.UWP
#elif __IOS__
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
#elif WINDOWS_UWP
				var native = new NativePainterView();
#endif
				SetNativeControl(native);
			}

			if (e.OldElement != null)
			{
				e.OldElement.GetJsonEvent -= HandleGetJson;
				e.OldElement.ClearEvent -= ClearImage;
				e.OldElement.LoadJsonEvent -= LoadJson;
				e.NewElement.SetStrokeColorEvent -= SetStrokeColorEvent;
				e.NewElement.GetStrokeColorEvent -= GetStrokeColorEvent;
				e.NewElement.SetStrokeThicknessEvent -= SetStrokeThicknessEvent;
				e.NewElement.GetStrokeThicknessEvent -= GetStrokeThicknessEvent;
				e.NewElement.SetImagePathEvent -= SetImagePathEvent;
			}
			if (e.NewElement != null)
			{
				e.NewElement.GetJsonEvent += HandleGetJson;
				e.NewElement.ClearEvent += ClearImage;
				e.NewElement.LoadJsonEvent += LoadJson;
				e.NewElement.SetStrokeColorEvent += SetStrokeColorEvent;
				e.NewElement.GetStrokeColorEvent += GetStrokeColorEvent;
				e.NewElement.SetStrokeThicknessEvent += SetStrokeThicknessEvent;
				e.NewElement.GetStrokeThicknessEvent += GetStrokeThicknessEvent;
				e.NewElement.SetImagePathEvent += SetImagePathEvent;
			}

			e.NewElement.RendererInitialized();
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

		private void SetStrokeColorEvent(object sender, PainterView.ColorHandler e)
		{
			Control.StrokeColor = e.setColor;
		}

		private void GetStrokeColorEvent(object sender, PainterView.ColorHandler e)
		{
			e.getColor = Task.Run(() => Control.StrokeColor);
		}

		private void SetStrokeThicknessEvent(object sender, PainterView.ThicknessHandler e)
		{
			Control.StrokeThickness = e.setThickness;
		}

		private void GetStrokeThicknessEvent(object sender, PainterView.ThicknessHandler e)
		{
			e.getThickness = Task.Run(() => Control.StrokeThickness);
		}

		private void SetImagePathEvent(object sender, PainterView.SetImageHandler e)
		{
			Control.LoadImage(e.Path, e.InResources);
		}
	}
}
