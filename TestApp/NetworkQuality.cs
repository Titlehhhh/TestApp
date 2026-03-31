using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp
{
	public enum NetworkQuality
	{
		Low,
		Medium,
		High,
		Excellent
	}

	public interface IProgressImage 
	{
		void OnStartLoad();
		void OnStopLoad(Exception? exception);
		void OnCanceled();
	}
}
