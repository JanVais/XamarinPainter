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
		internal event EventHandler<JsonImageEventHandler> GetJsonEvent;

		public Task<string> GetJson()
		{
			var args = new JsonImageEventHandler();
			GetJsonEvent?.Invoke(this, args);
			return args.Json;
		}

		internal class JsonImageEventHandler : EventArgs
		{
			public JsonImageEventHandler()
			{

			}

			public Task<string> Json { get; set; } = Task.FromResult<string>("");
		}
	}
}
