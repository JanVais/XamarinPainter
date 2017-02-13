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

#endif


[assembly: ExportRenderer(typeof(PainterView), typeof(PainterViewRenderer))]

#if WINDOWS_PHONE
namespace Painter.Forms.WindowsPhone
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
#else
				var native = new NativePainterView(new CoreGraphics.CGRect(0, 0, e.NewElement.Bounds.Width, e.NewElement.Bounds.Height));
				native.Opaque = false;
#endif
				SetNativeControl(native);
			}

			if (e.OldElement != null)
			{
				e.OldElement.GetJsonEvent -= HandleGetJson;
				e.OldElement.ClearEvent -= ClearImage;
				e.OldElement.LoadJsonEvent -= LoadJson;
			}
			if (e.NewElement != null)
			{
				e.NewElement.GetJsonEvent += HandleGetJson;
				e.NewElement.ClearEvent += ClearImage;
				e.NewElement.LoadJsonEvent += LoadJson;
			}
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
	}
}
