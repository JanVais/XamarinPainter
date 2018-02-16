//
// SaveAndLoad.cs
//
// Author:
//     Miley Hollenberg
//
// Copyright (c) 2017 Nitrocrime
//
//
using System;
using Xamarin.Forms;
using PainterTestbed.iOS;
using System.IO;
using System.Threading.Tasks;
using PainterTestbed;
using Foundation;
using System.Linq;

[assembly: Dependency(typeof(SaveAndLoad_iOS))]

namespace PainterTestbed.iOS
{
	public class SaveAndLoad_iOS : ISaveAndLoad
	{
		public static string DocumentsPath
		{
			get
			{
				var documentsDirUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last();
				return documentsDirUrl.Path;
			}
		}

		public string GetPathForFile(string fileName)
		{
			return CreatePathToFile(fileName);
		}

		public async Task SaveTextAsync(string filename, string text)
		{
			string path = CreatePathToFile(filename);
			using (StreamWriter sw = File.CreateText(path))
				await sw.WriteAsync(text);
		}

		public async Task<string> LoadTextAsync(string filename)
		{
			string path = CreatePathToFile(filename);
			if (FileExists(path))
			{
				using (StreamReader sr = File.OpenText(path))
					return await sr.ReadToEndAsync();
			}
			else
				return null;
		}

		public bool FileExists(string filename)
		{
			return File.Exists(CreatePathToFile(filename));
		}

		static string CreatePathToFile(string fileName)
		{
			return Path.Combine(DocumentsPath, fileName);
		}

        public byte[] GetFileBinary(string filePath)
        {
            try
            {
                filePath = filePath.Replace("file:", "");
                var bytes = File.ReadAllBytes(filePath);
                return bytes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return null;
        }

        public byte[] GetFileBinary(string filePath, bool resourceFile)
        {
            //TODO implement resource file
            try
            {
                NSData data = NSData.FromFile(CreatePathToFile(filePath));
                return data.ToArray();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void SaveFile(byte[] data, string url)
        {
            try
            {
                NSData finalData = NSData.FromArray(data);
                finalData.Save(NSUrl.FromFilename(CreatePathToFile(url)), true);
                finalData = null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}