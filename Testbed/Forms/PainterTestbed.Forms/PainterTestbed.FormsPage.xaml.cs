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
        private float alpha = 1.0f;

        public PainterTestbed_FormsPage()
		{
			InitializeComponent();

            painterView.StrokeColor = new Painter.Abstractions.Color(0, 1, 0, 0.5);
			painterView.StrokeThickness = 10;
			painterView.Initialized += (sender, e) =>
			{
                if (DependencyService.Get<ISaveAndLoad>().FileExists("background.jpg"))
                {
                    painterView.LoadImage(DependencyService.Get<ISaveAndLoad>().GetPathForFile("background.jpg"), false, Painter.Abstractions.Scaling.Absolute_Fit);
					var size = painterView.GetImageSize();
                    int rotation = painterView.GetImageOrientation();
                    Debug.WriteLine(rotation);
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            painterView.FinishedStrokeEvent += PainterView_GetFinishedData;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            painterView.FinishedStrokeEvent -= PainterView_GetFinishedData;
        }

        private void PainterView_GetFinishedData(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(sender);
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
            //var background = DependencyService.Get<ISaveAndLoad>().GetFileBinary(DependencyService.Get<ISaveAndLoad>().GetPathForFile("background.jpg"), false);
            
            var data = await export.GetCurrentImageAsPNG((int)painterView.Width, (int)painterView.Height, painterView.GetStrokes(), Painter.Abstractions.Scaling.Relative_Fit, 80, new Painter.Abstractions.Color(1, 1, 1, 1), true, null);

            DependencyService.Get<ISaveAndLoad>().SaveFile(data, "image.png");

            Debug.WriteLine(data.Length);
        }

		private async void LoadJson(object sender, System.EventArgs e)
		{
			var data = await DependencyService.Get<ISaveAndLoad>().LoadTextAsync("image.json");
			painterView.LoadJson(data);
		}

		private void setRedColor(object sender, EventArgs e)
		{
			painterView.StrokeColor = new Painter.Abstractions.Color(1, 0, 0, alpha);
		}

		private void setGreenColor(object sender, EventArgs e)
		{
            painterView.StrokeColor = new Painter.Abstractions.Color(0, 1, 0, alpha);
		}

		private void setBlueColor(object sender, EventArgs e)
		{
            painterView.StrokeColor = new Painter.Abstractions.Color(0, 0, 1, alpha);
		}

		private void StepperChanged(object sender, EventArgs e)
		{
			painterView.StrokeThickness = (int)stepper.Value;
		}

        private void debugClick(object sender, EventArgs e)
        {
            if (alpha != 1.0f)
            {
                painterView.StrokeColor.A = alpha = 1.0f;
                painterView.StrokeThickness = 2;
            }
            else
            {
                painterView.StrokeColor.A = alpha = 0.7f;
                painterView.StrokeThickness = 10;
            }

        }
    }
}
