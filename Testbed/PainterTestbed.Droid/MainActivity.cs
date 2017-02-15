using Android.App;
using Android.Widget;
using Android.OS;
using Painter.Android;
using Java.Interop;
using Android.Views;
using System.IO;

namespace PainterTestbed.Droid
{
	[Activity(Label = "PainterTestbed.Droid", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		PainterView painter;
		TextView stepper_lbl;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			painter = (PainterView)FindViewById(Resource.Id.painterView1);
			stepper_lbl = (TextView)FindViewById(Resource.Id.stepper_lbl);
		}

		[Export("saveJson")]
		public void saveJson(View v)
		{
			try
			{
				string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				string filename = Path.Combine(path, "image.json");

				using (var streamWriter = new StreamWriter(filename, false))
				{
					streamWriter.Write(painter.GetJson());
				}
			}
			catch (System.Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

		[Export("clearImage")]
		public void clearImage(View v)
		{
			painter.Clear();
		}

		[Export("loadJson")]
		public void loadJson(View v)
		{
			string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
			string filename = Path.Combine(path, "image.json");

			using (var streamReader = new StreamReader(filename))
			{
				string content = streamReader.ReadToEnd();
				painter.LoadJson(content);
			}
		}

		[Export("setRedColor")]
		public void setRedColor(View v)
		{
			painter.StrokeColor = new Painter.Abstractions.Color(1, 0, 0);
		}

		[Export("setGreenColor")]
		public void setGreenColor(View v)
		{
			painter.StrokeColor = new Painter.Abstractions.Color(0, 1, 0);
		}

		[Export("setBlueColor")]
		public void setBlueColor(View v)
		{
			painter.StrokeColor = new Painter.Abstractions.Color(0, 0, 1);
		}

		[Export("stepperSubtract")]
		public void stepperSubtract(View v)
		{
			painter.StrokeThickness--;
			if (painter.StrokeThickness <= 0.0)
				painter.StrokeThickness = 1.0;
			stepper_lbl.Text = painter.StrokeThickness.ToString("##");
		}

		[Export("stepperAdd")]
		public void stepperAdd(View v)
		{
			painter.StrokeThickness++;
			if (painter.StrokeThickness >= 100.0)
				painter.StrokeThickness = 100.0;
			stepper_lbl.Text = painter.StrokeThickness.ToString("##");
		}
	}
}

