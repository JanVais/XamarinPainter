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

		public event EventHandler GetFinishedData;

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

		public void SetEventHandler(EventHandler eventHandler)
		{
			this.customEventHandler = eventHandler;
		}

		public EventHandler customEventHandler;

		internal event EventHandler<SaveJsonImageHandler> GetJsonEvent;
		internal event EventHandler ClearEvent;
		internal event EventHandler<LoadJsonEventHandler> LoadJsonEvent;
		internal event EventHandler<SetImageHandler> SetImagePathEvent;
		internal event EventHandler<StrokesHandler> GetStrokesEvent;
		public EventHandler<SaveJsonImageHandler> FinishEventHandler;


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
	}
}
