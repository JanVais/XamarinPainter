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
		UIKit.UIStepper stepper { get; set; }

		[Outlet]
		UIKit.UILabel stepper_lbl { get; set; }

		[Outlet]
		Painter.iOS.PainterView v1 { get; set; }

		[Outlet]
		Painter.iOS.PainterView v2 { get; set; }

		[Outlet]
		Painter.iOS.PainterView v3 { get; set; }

		[Action ("clear:")]
		partial void clear (Foundation.NSObject sender);

		[Action ("loadJson:")]
		partial void loadJson (Foundation.NSObject sender);

		[Action ("saveJson:")]
		partial void saveJson (Foundation.NSObject sender);

		[Action ("setBlueColor:")]
		partial void setBlueColor (Foundation.NSObject sender);

		[Action ("setGreenColor:")]
		partial void setGreenColor (Foundation.NSObject sender);

		[Action ("setRedColor:")]
		partial void setRedColor (Foundation.NSObject sender);

		[Action ("setStepperValue:")]
		partial void setStepperValue (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (stepper != null) {
				stepper.Dispose ();
				stepper = null;
			}

			if (stepper_lbl != null) {
				stepper_lbl.Dispose ();
				stepper_lbl = null;
			}

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
