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
				return Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
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

		public bool FileExists(string filename)
		{
			return File.Exists(CreatePathToFile(filename));
		}

		static string CreatePathToFile(string fileName)
		{
			return Path.Combine(DocumentsPath, fileName);
		}

        public byte[] GetFileBinary(string filePath, bool resourceFile)
        {
            if(resourceFile)
            {
                var assembly = this.GetType().GetTypeInfo().Assembly; // you can replace "this.GetType()" with "typeof(MyType)", where MyType is any type in your assembly.
                byte[] buffer;
                using (Stream s = assembly.GetManifestResourceStream(filePath))
                {
                    if(s == null)
                    {
                        return null;
                    }

                    long length = s.Length;
                    buffer = new byte[length];
                    s.Read(buffer, 0, (int)length);
                }
                return buffer;
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
    }
}