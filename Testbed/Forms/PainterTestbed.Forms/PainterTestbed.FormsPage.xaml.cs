//
// PainterTestbed.FormsPage.xaml.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System.Diagnostics;
using Painter.Forms;
using Xamarin.Forms;

namespace PainterTestbed.Forms
{
	public partial class PainterTestbed_FormsPage : ContentPage
	{
		public PainterTestbed_FormsPage()
		{
			InitializeComponent();
		}

		private async void Handle_Clicked(object sender, System.EventArgs e)
		{
			var data = await painterView.GetJson();

			Debug.WriteLine(data);
		}
	}
}
