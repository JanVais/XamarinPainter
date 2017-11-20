//
// PainterTestbed.FormsPage.xaml.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using System.Diagnostics;
using System.IO;
using Painter.Forms;
using PainterTestbed;
using Xamarin.Forms;
using Painter.Interfaces;

namespace PainterTestbed.Forms
{
	public partial class PainterTestbed_FormsPage : ContentPage
	{
        public PainterTestbed_FormsPage()
		{
			InitializeComponent();

            painterView.StrokeColor = new Painter.Abstractions.Color(0, 1, 0, 0.5);
			painterView.StrokeThickness = 10;
			painterView.Initialized += (sender, e) =>
			{
				//if (DependencyService.Get<ISaveAndLoad>().FileExists("light.jpeg"))
				//	painterView.LoadImage(DependencyService.Get<ISaveAndLoad>().GetPathForFile("light.jpeg"), false, Painter.Abstractions.Scaling.Absolute_Fit);
			};

            painterView.FinishedStrokeEvent = PainterView_GetFinishedData;
        }

        private void PainterView_GetFinishedData(object sender, EventArgs e)
        {
            // Done with saving etc
            // do what you want..
		}

		private void clearDrawing()
		{
			painterView.Clear();
		}

        private async void SaveJson(object sender, System.EventArgs e)
		{
			var data = await painterView.GetJson();
			await DependencyService.Get<ISaveAndLoad>().SaveTextAsync("image.json", data);
		}
        
        private async void SaveImage()
        {
            IPainterExport export = DependencyService.Get<IPainterExport>();
            var background = DependencyService.Get<ISaveAndLoad>().GetFileBinary("light.jpeg", true);
            
            var data = await export.ExportCurrentImage((int)painterView.Width, (int)painterView.Height, painterView.GetStrokes(), Painter.Abstractions.Scaling.Relative_Fit, Painter.Abstractions.ExportFormat.Png, 80, new Painter.Abstractions.Color(1, 1, 1, 1), true, background);
            
            DependencyService.Get<ISaveAndLoad>().SaveFile(data, "image.png");

            Debug.WriteLine(data.Length);
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
			painterView.StrokeColor = new Painter.Abstractions.Color(1, 0, 0, 0.2);
		}

		private void setGreenColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(0, 1, 0, 0.2);
		}

		private void setBlueColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(0, 0, 1, 0.2);
		}

		private void StepperChanged(object sender, EventArgs e)
		{
			painterView.StrokeThickness = (int)stepper.Value;
		}
	}
}
