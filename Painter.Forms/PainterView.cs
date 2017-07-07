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

namespace Painter.Forms
{
	public class PainterView : View
	{
		public event EventHandler Initialized;
		internal void RendererInitialized()
		{
			Initialized?.Invoke(this, null);
		}

		internal event EventHandler<SaveJsonImageHandler> GetJsonEvent;
		internal event EventHandler ClearEvent;
		internal event EventHandler<LoadJsonEventHandler> LoadJsonEvent;
		internal event EventHandler<ColorHandler> SetStrokeColorEvent;
		internal event EventHandler<ColorHandler> GetStrokeColorEvent;
		internal event EventHandler<ThicknessHandler> SetStrokeThicknessEvent;
		internal event EventHandler<ThicknessHandler> GetStrokeThicknessEvent;
		internal event EventHandler<SetImageHandler> SetImagePathEvent;
        internal event EventHandler<StrokesHandler> GetStrokesEvent;

		public Abstractions.Color StrokeColor
		{
			get
			{
				var args = new ColorHandler();
				GetStrokeColorEvent?.Invoke(this, args);
				return args.getColor.Result;
			}
			set
			{
				var args = new ColorHandler() { setColor = value };
				SetStrokeColorEvent?.Invoke(this, args);
			}
		}

		public double StrokeThickness
		{
			get
			{
				var args = new ThicknessHandler();
				GetStrokeThicknessEvent?.Invoke(this, args);
				return args.getThickness.Result;
			}
			set
			{
				var args = new ThicknessHandler() { setThickness = value };
				SetStrokeThicknessEvent?.Invoke(this, args);
			}
		}

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

		public void LoadImage(string imageUri, bool InResources = true)
		{
			var args = new SetImageHandler() { Path = imageUri, InResources = InResources };
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

		internal class SaveJsonImageHandler : EventArgs
		{
			public Task<string> Json { get; set; } = Task.FromResult<string>("");
		}

		internal class LoadJsonEventHandler : EventArgs
		{
			public string Json { get; set; }
		}

		internal class ColorHandler : EventArgs
		{
			public Abstractions.Color setColor { get; set; }
			public Task<Abstractions.Color> getColor { get; set; } = Task.FromResult<Abstractions.Color>(new Abstractions.Color());
		}

		internal class ThicknessHandler : EventArgs
		{
			public double setThickness { get; set; }
			public Task<double> getThickness { get; set; } = Task.FromResult<double>(1.0);
		}

		internal class SetImageHandler : EventArgs
		{
			public string Path { get; set; }
			public bool InResources { get; set; }
		}

        internal class StrokesHandler : EventArgs
        {
            public Task<List<Stroke>> GetStrokes { get; set; } = Task.FromResult<List<Stroke>>(null);
            //TODO Set
        }
    }
}
