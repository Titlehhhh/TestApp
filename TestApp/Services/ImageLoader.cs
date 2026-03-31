using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Imaging;

namespace TestApp.Services
{
	public static class ImageLoader
	{
		public static NetworkQuality Quality { get; set; }

		private readonly static HttpClient httpClient = new()
		{
			Timeout = TimeSpan.FromSeconds(30)
		};
		public static async Task<BitmapImage> LoadAsync(
			Uri uri,
			IProgress<double>? progress = null,
			CancellationToken cancellationToken = default)
		{
			progress?.Report(0.0);
			var quality = Quality;
			Debug.WriteLine($"Starting download with network quality: {quality}");
			using var response =
				await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

			response.EnsureSuccessStatusCode();

			var length = response.Content.Headers.ContentLength;

			if (length <= 0)
			{
				throw new InvalidOperationException("Content length is not specified.");
			}

			var buffer = ArrayPool<byte>.Shared.Rent(1024 * 64);

			var ms = new MemoryStream();
			using var source =
				await response.Content.ReadAsStreamAsync(cancellationToken);

			
			try
			{
				while (true)
				{
					var read = await source.ReadAsync(buffer.AsMemory(0, 1024 * 64), cancellationToken);

					await RandomTimeout(quality);

					if (read <= 0)
					{
						break;
					}

					ms.Write(buffer, 0, read);

					if (progress is not null)
					{
						if (length > 0)
						{
							double progressValue = (double)ms.Length / length.Value;
							progress.Report(progressValue);
						}
						else
						{
							progress.Report(0.0);
						}
					}
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}

			progress?.Report(1.0);
			ms.Position = 0;



			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = ms;
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();
			bitmap.Freeze();

			return bitmap;
		}

		private static async ValueTask RandomTimeout(NetworkQuality quality)
		{
			int msTime = 0;
			if (quality == NetworkQuality.Excellent)
			{
				return;
			}
			else if (quality == NetworkQuality.High)
			{
				msTime = Random.Shared.Next(100, 500);
			}
			else if (quality == NetworkQuality.Medium)
			{
				msTime = Random.Shared.Next(500, 1500);
			}
			else if (quality == NetworkQuality.Low)
			{
				msTime = Random.Shared.Next(1500, 3000);
			}
			await Task.Delay(msTime);
		}
	}
}