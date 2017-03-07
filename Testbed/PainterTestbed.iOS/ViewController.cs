//
// ViewController.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Foundation;
using Painter;
using Painter.Abstractions;
using Painter.iOS;
using UIKit;

namespace PainterTestbed.iOS
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.


		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			v1.LoadImage("background.jpg");
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		partial void saveJson(NSObject sender)
		{
			try
			{
				var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var filename = Path.Combine(path, "image.json");
				File.WriteAllText(filename, v1.GetJson());
			}
			catch (Exception e)
			{

			}
		}

		partial void loadJson(NSObject sender)
		{
			try
			{
				var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				var filename = Path.Combine(path, "image.json");
				var json = File.ReadAllText(filename);
				v1.LoadJson(json);
			}
			catch (Exception e)
			{
				
			}
		}

		partial void clear(NSObject sender)
		{
			v1.Clear();
		}

		partial void setBlueColor(NSObject sender)
		{
			v1.StrokeColor = new Color(0, 0, 1);
		}

		partial void setGreenColor(NSObject sender)
		{
			v1.StrokeColor = new Color(0, 1, 0);
		}

		partial void setRedColor(NSObject sender)
		{
			v1.StrokeColor = new Color(1, 0, 0);
		}

		partial void setStepperValue(NSObject sender)
		{
			stepper_lbl.Text = stepper.Value.ToString("##");
			v1.StrokeThickness = stepper.Value;
		}
	}
}
