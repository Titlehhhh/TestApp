using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
			var slot = new ImageSlotViewModel(name, this);
			slot.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(ImageSlotViewModel.Progress))
				{
					OnPropertyChanged(nameof(Progress));
				}
			};
			return slot;
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
		private int _startedCount = 0;
		private int _finishedCount = 0;
		private int _loadingCount = 0;

		public void OnStartLoad()
		{
			lock (_sync)
			{
				_loadingCount++;
				_startedCount++;
			}
			UpdateProgress();
		}

		public void OnStopLoad(Exception? exception)
		{
			lock (_sync)
			{
				_loadingCount--;
				_finishedCount++;
				TryReset();
			}
			UpdateProgress();
		}

		public void OnCanceled()
		{
			lock (_sync)
			{
				_loadingCount--;
				_finishedCount++;
				TryReset();
			}
			UpdateProgress();
		}

		private void TryReset()
		{
			if (_loadingCount == 0)
			{
				_startedCount = 0;
				_finishedCount = 0;
			}
		}

		public double Progress
		{
			get
			{
				var running = Slots.Where(s => s.LoadCommand.IsRunning).ToList();
				if (running.Count == 0) return 0;
				return running.Sum(s => s.Progress) / running.Count;
			}
		}

		public string ProgressText
		{
			get
			{
				lock (_sync)
				{
					if (_loadingCount == 0) return string.Empty;
					return $"{_finishedCount} из {_startedCount}";
				}
			}
		}

		private void UpdateProgress()
		{
			OnPropertyChanged(nameof(Progress));
			OnPropertyChanged(nameof(ProgressText));
		}


	}
}
