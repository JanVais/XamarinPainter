//
// ISaveAndLoad.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime 2017
//
//
using System;
using System.Threading.Tasks;

namespace PainterTestbed
{
	public interface ISaveAndLoad
	{
		Task SaveTextAsync(string filename, string text);
		Task<string> LoadTextAsync(string filename);
		bool FileExists(string filename);
	}
}
