// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace PainterTestbed.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		Painter.iOS.PainterView v1 { get; set; }

		[Outlet]
		Painter.iOS.PainterView v2 { get; set; }

		[Outlet]
		Painter.iOS.PainterView v3 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (v1 != null) {
				v1.Dispose ();
				v1 = null;
			}

			if (v2 != null) {
				v2.Dispose ();
				v2 = null;
			}

			if (v3 != null) {
				v3.Dispose ();
				v3 = null;
			}
		}
	}
}
