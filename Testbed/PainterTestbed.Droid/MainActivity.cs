using Android.App;
using Android.Widget;
using Android.OS;
using Painter.Android;
using Java.Interop;
using Android.Views;
using System.IO;
using System;
using Java.IO;

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

        [Export("saveImage")]
        public async void saveImage(View v)
        {
            try
            {
                var data_fit = await painter.GetCurrentImageAsJPG(painter.Width * 2, painter.Height * 3, scaling: Painter.Abstractions.Scaling.Absolute_Fit);
                var data_fill = await painter.GetCurrentImageAsJPG(painter.Width * 2, painter.Height * 3, scaling: Painter.Abstractions.Scaling.Absolute_Fill);
                var data_none = await painter.GetCurrentImageAsJPG(painter.Width * 2, painter.Height * 3, scaling: Painter.Abstractions.Scaling.Absolute_None);

                var storageDir = Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
                Java.IO.File file_fit = new Java.IO.File(storageDir, "image_" + DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds + "_fit.jpg");
                Java.IO.File file_fill = new Java.IO.File(storageDir, "image_" + DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds + "_fill.jpg");
                Java.IO.File file_none = new Java.IO.File(storageDir, "image_" + DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds + "_none.jpg");

                FileOutputStream stream_fit = new FileOutputStream(file_fit);
                stream_fit.Write(data_fit);
                stream_fit.Close();

                FileOutputStream stream_fill = new FileOutputStream(file_fill);
                stream_fill.Write(data_fill);
                stream_fill.Close();

                FileOutputStream stream_none = new FileOutputStream(file_none);
                stream_none.Write(data_none);
                stream_none.Close();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
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

