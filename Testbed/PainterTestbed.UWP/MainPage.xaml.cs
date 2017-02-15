using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PainterTestbed.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

			ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
		}

		private async void SaveJson(object sender, TappedRoutedEventArgs e)
		{
			Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			Windows.Storage.StorageFile ticketsFile = await storageFolder.CreateFileAsync("image.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
			await Windows.Storage.FileIO.WriteTextAsync(ticketsFile, painterView.GetJson());
		}

		private async void Clear(object sender, TappedRoutedEventArgs e)
		{
			await painterView.Clear();
		}

		private async void LoadJson(object sender, TappedRoutedEventArgs e)
		{
			Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			Windows.Storage.StorageFile ticketsFile = await storageFolder.CreateFileAsync("image.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
			string json = await Windows.Storage.FileIO.ReadTextAsync(ticketsFile);
			painterView.LoadJson(json);
		}
	}
}
