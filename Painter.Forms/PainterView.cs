//
// PainterView.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Threading.Tasks;
using Painter;
using Xamarin.Forms;

namespace Painter.Forms
{
	public class PainterView : View
	{
		internal event EventHandler<SaveJsonImageHandler> GetJsonEvent;
		internal event EventHandler ClearEvent;
		internal event EventHandler<LoadJsonEventHandler> LoadJsonEvent;


		public Task<string> GetJson()
		{
			var args = new SaveJsonImageHandler();
			GetJsonEvent?.Invoke(this, args);
			return args.Json;
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
	}
}
