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
		public ImageSlotViewModel Slot1 { get; }
		public ImageSlotViewModel Slot2 { get; }
		public ImageSlotViewModel Slot3 { get; }

		public MainViewModel()
		{
			Slot1 = new("Image 1");
			Slot2 = new("Image 2");
			Slot3 = new("Image 3");
		}

		
	}
}
