using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestApp.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly static HttpClient _httpClient = new();

		[ObservableProperty]
		public partial ImageSource Image1 { get; set; }

		[ObservableProperty]
		public partial string Image1Uri { get; set; }

		[ObservableProperty]
		public partial string Image1Status { get; set; }

		public MainViewModel()
		{

		}

		[RelayCommand]
		public async Task LoadImage1()
		{
			if (string.IsNullOrWhiteSpace(Image1Uri))
				return;

			if (!Uri.TryCreate(Image1Uri, UriKind.Absolute, out var uri))
			{
				//Todo show error
				return;
			}



			var stream = await _httpClient.GetStreamAsync(uri).ConfigureAwait(false);
			
			var ms = new MemoryStream();
			await stream.CopyToAsync(ms);
			ms.Position = 0;

			BitmapImage img = new();
			img.BeginInit();
			img.StreamSource = ms;
			img.CacheOption = BitmapCacheOption.OnLoad;
			img.EndInit();
			img.Freeze();

			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				Image1 = img;
				Image1Status = $"Loaded {uri}";
			});
		}
	}
}
