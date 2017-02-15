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
using PainterTestbed;
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

		private void setRedColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(1, 0, 0);
		}

		private void setGreenColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(0, 1, 0);
		}

		private void setBlueColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(0, 0, 1);
		}

		private void StepperChanged(object sender, EventArgs e)
		{
			painterView.StrokeThickness = stepper.Value;
		}
	}
}
