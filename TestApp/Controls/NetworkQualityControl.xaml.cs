using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp.Controls
{
	/// <summary>
	/// Логика взаимодействия для UserControl1.xaml
	/// </summary>
	public partial class NetworkQualityControl : UserControl
	{
		public static readonly DependencyProperty QualityProperty =
			DependencyProperty.Register(
				nameof(Quality),
				typeof(NetworkQuality),
				typeof(NetworkQualityControl),
				new PropertyMetadata(NetworkQuality.High, OnQualityChanged));
		private static void OnQualityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (NetworkQualityControl)d;
			control.UpdateButtons((NetworkQuality)e.NewValue);
		}

		private void UpdateButtons(NetworkQuality quality)
		{
			LowRB.IsChecked = quality == NetworkQuality.Low;
			MediumRB.IsChecked = quality == NetworkQuality.Medium;
			HighRB.IsChecked = quality == NetworkQuality.High;
			ExcellentRB.IsChecked = quality == NetworkQuality.Excellent;
		}
		public NetworkQuality Quality
		{
			get => (NetworkQuality)GetValue(QualityProperty);
			set => SetValue(QualityProperty, value);
		}
		public NetworkQualityControl()
		{
			InitializeComponent();
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			var radioButton = sender as RadioButton;
			if (radioButton is null)
				return;

			if (radioButton.Tag is not string tag)
				return;

			var quality = tag switch
			{
				"Низкое" => NetworkQuality.Low,
				"Среднее" => NetworkQuality.Medium,
				"Высокое" => NetworkQuality.High,
				"Отличное" => NetworkQuality.Excellent,
				_ => throw new InvalidOperationException("Invalid tag value.")
			};
			Quality = quality;
		}



	}


}
