//
// AppDelegate.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Painter.Forms.iOS;
using UIKit;

namespace PainterTestbed.Forms.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			PainterRenderer.Init();

			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
