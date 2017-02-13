using Android.App;
using Android.Widget;
using Android.OS;
using Painter.Android;

namespace PainterTestbed.Droid
{
	[Activity(Label = "PainterTestbed.Droid", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			PainterView v = (PainterView)FindViewById(Resource.Id.painterView1);


		}
	}
}

