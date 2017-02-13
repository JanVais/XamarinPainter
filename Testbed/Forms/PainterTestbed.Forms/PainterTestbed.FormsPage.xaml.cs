//
// PainterTestbed.FormsPage.xaml.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Diagnostics;
using System.IO;
using Painter.Forms;
using WorkingWithFiles;
using Xamarin.Forms;

namespace PainterTestbed.Forms
{
	public partial class PainterTestbed_FormsPage : ContentPage
	{
		public PainterTestbed_FormsPage()
		{
			InitializeComponent();
		}

		private async void SaveJson(object sender, System.EventArgs e)
		{
			var data = await painterView.GetJson();
			await DependencyService.Get<ISaveAndLoad>().SaveTextAsync("image.json", data);
		}

		private void Clear(object sender, System.EventArgs e)
		{
			painterView.Clear();
		}

		private async void LoadJson(object sender, System.EventArgs e)
		{
			var data = await DependencyService.Get<ISaveAndLoad>().LoadTextAsync("image.json");
			painterView.LoadJson(data);
		}
	}
}
