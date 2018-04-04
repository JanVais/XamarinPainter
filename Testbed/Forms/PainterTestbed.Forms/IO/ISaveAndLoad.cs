//
// ISaveAndLoad.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
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
        byte[] GetFileBinary(string filePath, bool resourceFile);
        void SaveFile(byte[] data, string url);
		string GetPathForFile(string fileName);
    }
}
