using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PainterTestbed;
using Xamarin.Forms;
using PainterTestbed.Forms.UWP;

[assembly: Dependency(typeof(SaveAndLoad_UWP))]

namespace PainterTestbed.Forms.UWP
{
	class SaveAndLoad_UWP : ISaveAndLoad
	{
		public bool FileExists(string filename)
		{
			Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			Windows.Storage.StorageFile ticketsFile = storageFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.OpenIfExists).GetResults();
			return ticketsFile != null;
		}

		public async Task<string> LoadTextAsync(string filename)
		{
			Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			Windows.Storage.StorageFile ticketsFile = storageFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.OpenIfExists).GetResults();
			return await Windows.Storage.FileIO.ReadTextAsync(ticketsFile);
		}

		public async Task SaveTextAsync(string filename, string text)
		{
			Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			Windows.Storage.StorageFile ticketsFile = await storageFolder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);
			await Windows.Storage.FileIO.WriteTextAsync(ticketsFile, text);
		}
	}
}
