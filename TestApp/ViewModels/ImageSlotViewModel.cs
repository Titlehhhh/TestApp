using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Media;
using TestApp.Services;

namespace TestApp.ViewModels
{
	public partial class ImageSlotViewModel : ObservableObject
	{
		private readonly IProgressImage _progressImage;
		public ImageSlotViewModel(string name, IProgressImage progressImage)
		{
			Name = name;
			_progressImage = progressImage;
		}

		[ObservableProperty]
		public partial string Name { get; private set; }

		[ObservableProperty]
		public partial ImageSource? Image { get; set; }

		[ObservableProperty]
		public partial string? Url { get; set; }

		[ObservableProperty]
		public partial string? Error { get; set; }

		[ObservableProperty]
		public partial double Progress { get; set; }


		[RelayCommand(IncludeCancelCommand = true, FlowExceptionsToTaskScheduler = false)]
		public async Task Load(CancellationToken cancellationToken)
		{

			Error = null;
			if (string.IsNullOrWhiteSpace(Url))
			{
				Error = "Введите URL!";
				return;
			}

			if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri))
			{
				Error = "Неправильный формат!";
				return;
			}
			_progressImage.OnStartLoad();

			var progress = new Progress<double>(v => Progress = v);

			try
			{
				Image = await ImageLoader.LoadAsync(uri, progress, cancellationToken);
				_progressImage.OnStopLoad(null);
			}
			catch (OperationCanceledException)
			{
				_progressImage.OnCanceled();
			}
			catch (Exception ex)
			{
				Error = "Не удалось загрузить изображение!";
				_progressImage.OnStopLoad(ex);
			}
		}
		

		partial void OnUrlChanged(string? value)
		{
			Error = null;
		}

	}


}
