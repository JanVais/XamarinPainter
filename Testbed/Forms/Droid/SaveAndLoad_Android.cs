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
using System.IO;
using System.Threading.Tasks;
using PainterTestbed;
using System.Linq;
using PainterTestbed.Droid;
using System.Reflection;

[assembly: Dependency(typeof(SaveAndLoad_Android))]

namespace PainterTestbed.Droid
{
	public class SaveAndLoad_Android : ISaveAndLoad
	{
		public static string DocumentsPath
		{
			get
			{
				return Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
			}
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

		public bool FileExists(string fileName)
		{
			return File.Exists(CreatePathToFile(fileName));
		}

		public string GetPathForFile(string fileName)
		{
			return CreatePathToFile(fileName);
		}

		static string CreatePathToFile(string fileName)
		{
			return Path.Combine(DocumentsPath, fileName);
		}

        public byte[] GetFileBinary(string filePath, bool resourceFile)
        {
            if(resourceFile)
            {
                try
                {
                    var assembly = this.GetType().GetTypeInfo().Assembly; // you can replace "this.GetType()" with "typeof(MyType)", where MyType is any type in your assembly.
                    var name = filePath.Split('.')[0];
                    var id = Android.App.Application.Context.Resources.GetIdentifier(name.ToLower(), "drawable", Android.App.Application.Context.PackageName);

                    var file = Android.Graphics.BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, id);

                    using (var stream = new System.IO.MemoryStream())
                    {
                        file.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                        return stream.ToArray();
                    }
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null;
                }
            }

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

        public void SaveFile(byte[] data, string url)
        {
            try
            {
                Java.IO.File f = new Java.IO.File(Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures), url);
                f.CreateNewFile();

                Java.IO.FileOutputStream fs = new Java.IO.FileOutputStream(f);
                fs.Write(data);
                fs.Close();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}