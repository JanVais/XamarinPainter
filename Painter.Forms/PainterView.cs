//
// PainterView.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using System.Threading.Tasks;
using Painter;
using Xamarin.Forms;
using System.Collections.Generic;
using Painter.Abstractions;
using System.ComponentModel;

namespace Painter.Forms
{
	public class PainterView : View, INotifyPropertyChanged
	{
		public new event PropertyChangedEventHandler PropertyChanged;
		override protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) // if there is any subscribers 
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event EventHandler Initialized;
		internal void RendererInitialized()
		{
			Initialized?.Invoke(this, null);
		}

		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
		   nameof(StrokeColor),
		   typeof(Abstractions.Color),
		   typeof(PainterView),
		   new Abstractions.Color(0, 0, 0, 1));

		public static readonly BindableProperty FinishedStrokeEventProperty = BindableProperty.Create(
			nameof(FinishedStrokeEvent),
			typeof(EventHandler),
			typeof(PainterView),
			null);

		public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create(
			nameof(StrokeThickness),
			typeof(int),
			typeof(PainterView),
			1);

        public static readonly BindableProperty BackgroundScalingProperty = BindableProperty.Create(
            nameof(BackgroundScaling),
            typeof(Abstractions.Scaling),
            typeof(PainterView),
            Abstractions.Scaling.Absolute_None);

        public static readonly BindableProperty CanvasWidthProperty = BindableProperty.Create(
            nameof(CanvasWidth),
            typeof(double),
            typeof(PainterView),
            -1.0);

        public static readonly BindableProperty CanvasHeightProperty = BindableProperty.Create(
            nameof(CanvasHeight),
            typeof(double),
            typeof(PainterView),
            -1.0);

		public EventHandler FinishedStrokeEvent
		{
			get
			{
				return (EventHandler)GetValue(FinishedStrokeEventProperty);
			}
			set
			{
				SetValue(FinishedStrokeEventProperty, value);
				OnPropertyChanged(nameof(FinishedStrokeEvent));
			}
		}

		public Abstractions.Color StrokeColor
		{
			get
			{
				return (Abstractions.Color)GetValue(StrokeColorProperty);
			}
			set
			{
				SetValue(StrokeColorProperty, value);
				OnPropertyChanged(nameof(StrokeColor));
			}
		}

		public int StrokeThickness
		{
			get
			{
				return (int)GetValue(StrokeThicknessProperty);
			}
			set
			{
				SetValue(StrokeThicknessProperty, value);
				OnPropertyChanged(nameof(StrokeThickness));
			}
		}

		public Abstractions.Scaling BackgroundScaling
		{
			get
			{
				return (Abstractions.Scaling)GetValue(BackgroundScalingProperty);
			}
			set
			{
				SetValue(BackgroundScalingProperty, value);
				OnPropertyChanged(nameof(BackgroundScalingProperty));
			}
		}

        public double CanvasWidth
        {
            get
            {
                return (double)GetValue(CanvasWidthProperty);
            }
        }

        public double CanvasHeight
        {
            get
            {
                return (double)GetValue(CanvasHeightProperty);
            }
        }


		internal event EventHandler<SaveJsonImageHandler> GetJsonEvent;
		internal event EventHandler ClearEvent;
		internal event EventHandler<LoadJsonEventHandler> LoadJsonEvent;
		internal event EventHandler<SetImageHandler> SetImagePathEvent;
		internal event EventHandler<StrokesHandler> GetStrokesEvent;
        internal event EventHandler<GetImageOrientationHandler> GetImageOrientationEvent;
        internal event EventHandler<GetImageSizeHandler> GetImageSizeEvent;
        internal event EventHandler<GetDrawingScaleHandler> GetDrawingScaleEvent;

        internal event EventHandler RotateTestEvent;
		
		public List<Stroke> GetStrokes()
		{
			var args = new StrokesHandler();
			GetStrokesEvent?.Invoke(this, args);
			return args.GetStrokes.Result;
		}

		public Task<string> GetJson()
		{
			var args = new SaveJsonImageHandler();
			GetJsonEvent?.Invoke(this, args);
			return args.Json;
		}

        public int GetImageOrientation()
        {
            var args = new GetImageOrientationHandler();
            GetImageOrientationEvent?.Invoke(this, args);
            return args.Orientation;
        }

        public float GetDrawingScale()
        {
            var args = new GetDrawingScaleHandler();
            GetDrawingScaleEvent?.Invoke(this, args);
            return args.Scale;
        }

        public Abstractions.Point GetImageSize(bool adjustedForDensity = false)
        {
            var args = new GetImageSizeHandler()
            {
                AdjustedForDensity = adjustedForDensity
            };
            GetImageSizeEvent?.Invoke(this, args);
            return args.ImageSize;
        }

		public void LoadImage(string imageUri, bool InResources = true, Abstractions.Scaling Scaling = Scaling.Absolute_None)
		{
			var args = new SetImageHandler() { Path = imageUri, InResources = InResources, Scaling = Scaling };
			SetImagePathEvent?.Invoke(this, args);
		}

		public void Clear()
		{
			ClearEvent?.Invoke(this, null);
		}

		public void LoadJson(string json)
		{
			LoadJsonEvent?.Invoke(this, new LoadJsonEventHandler() { Json = json });
		}

		public class SaveJsonImageHandler : EventArgs
		{
			public Task<string> Json { get; set; } = Task.FromResult<string>("");
		}

		internal class LoadJsonEventHandler : EventArgs
		{
			public string Json { get; set; }
		}

        internal class GetImageOrientationHandler : EventArgs
        {
            public int Orientation { get; set; }
        }

        internal class GetImageSizeHandler : EventArgs
        {
            public bool AdjustedForDensity { get; set; }
            public Abstractions.Point ImageSize { get; set; }
        }

		internal class SetBackgroundScalingHandler : EventArgs
		{
			public Abstractions.Scaling Scaling { get; set; }
		}

		internal class SetImageHandler : EventArgs
		{
			public string Path { get; set; }
			public bool InResources { get; set; }
			public Abstractions.Scaling Scaling { get; set; }
		}

		internal class StrokesHandler : EventArgs
		{
			public Task<List<Stroke>> GetStrokes { get; set; } = Task.FromResult<List<Stroke>>(null);
			//TODO Set
		}

        internal class GetDrawingScaleHandler : EventArgs
        {
            public float Scale { get; set; }
        }

        public void RotateTest()
        {
            RotateTestEvent?.Invoke(this, null);
        }

    }
}
