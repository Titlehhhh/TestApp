using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TestApp.Services;

namespace TestApp.ViewModels
{
	public partial class MainViewModel : ObservableObject, IProgressImage
	{
		public ImageSlotViewModel Slot1 { get; }
		public ImageSlotViewModel Slot2 { get; }
		public ImageSlotViewModel Slot3 { get; }


		public NetworkQuality Quality
		{
			get => field;
			set
			{
				if (field != value)
				{
					field = value;
					ImageLoader.Quality = value;
				}
			}
		}

		private IEnumerable<ImageSlotViewModel> Slots
		{
			get
			{
				yield return Slot1;
				yield return Slot2;
				yield return Slot3;
			}
		}
		public MainViewModel()
		{
			Slot1 = CreateSlot("Image 1");
			Slot2 = CreateSlot("Image 2");
			Slot3 = CreateSlot("Image 3");
			Quality = NetworkQuality.Excellent;
		}

		private ImageSlotViewModel CreateSlot(string name)
		{
			return new ImageSlotViewModel(name, this);
		}

		[RelayCommand]
		public void StartAll()
		{
			foreach (var item in Slots)
			{
				if (item.LoadCommand.CanExecute(null))
				{
					item.LoadCommand.Execute(null);
				}
			}
		}
		private readonly object _sync = new object();
		private int _loadingCount = 0;

		public void OnStartLoad()
		{
			lock (_sync)
				_loadingCount++;
			UpdateProgress();
		}

		public void OnStopLoad(Exception? exception)
		{
			lock (_sync)
				_loadingCount--;
			UpdateProgress();
		}

		public void OnCanceled()
		{
			lock (_sync)
				_loadingCount--;
			UpdateProgress();
		}

		public int Progress
		{
			get { lock (_sync) return _loadingCount; }
		}

		private void UpdateProgress()
		{
			OnPropertyChanged(nameof(Progress));
		}


	}
}
