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

			const int maxAttempts = 3;
			HttpResponseMessage? response = null;

			for (var attempt = 0; attempt < maxAttempts; attempt++)
			{
				try
				{
					response = await httpClient
						.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
						.ConfigureAwait(false);
					break;
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (HttpRequestException ex) when (ex.HttpRequestError == HttpRequestError.ConnectionError)
				{
					Debug.WriteLine($"Connection error on attempt {attempt + 1}: {ex.Message}");

					if (attempt == maxAttempts - 1)
						throw;

					await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
				}
			}

			response!.EnsureSuccessStatusCode();

			var length = response.Content.Headers.ContentLength;

			using var source = await response.Content.ReadAsStreamAsync(cancellationToken);

			var ms = new MemoryStream();

			var buffer = ArrayPool<byte>.Shared.Rent(1024 * 64);
			try
			{
				int read;
				while ((read = await source.ReadAsync(buffer.AsMemory(0, 1024 * 64), cancellationToken)) > 0)
				{
					ms.Write(buffer, 0, read);
					await RandomTimeout(quality, cancellationToken);

					if (length.HasValue)
						progress?.Report((double)ms.Length / length.Value);
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
				response.Dispose();
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

		private static async ValueTask RandomTimeout(NetworkQuality quality, CancellationToken cancellationToken)
		{
			var ms = quality switch
			{
				NetworkQuality.Excellent => 0,
				NetworkQuality.High => Random.Shared.Next(100, 500),
				NetworkQuality.Medium => Random.Shared.Next(500, 1500),
				NetworkQuality.Low => Random.Shared.Next(1500, 3000),
				_ => 0
			};

			if (ms > 0)
				await Task.Delay(ms, cancellationToken);
		}
	}
}