using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace TestApp.ViewModels
{
	public partial class ImageSlotViewModel : ObservableObject
	{
		public ImageSlotViewModel(string name)
		{
			Name = name;
		}

		[ObservableProperty]
		public partial string Name { get; private set; }

		[ObservableProperty]
		public partial ImageSource Image { get; set; }

		[ObservableProperty]
		public partial string? Url { get; set; }

		[ObservableProperty]
		public partial string? Error { get; set; }

		[RelayCommand]
		public async Task Start()
		{
			if (string.IsNullOrWhiteSpace(Url))
			{
				return;
			}

			if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri))
			{
				Error = "Неправильный формат!";
				return;
			}


			
		}

		[RelayCommand]
		public void Stop()
		{

		}
	}
}
