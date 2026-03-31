namespace TestApp
{
	public interface IProgressImage 
	{
		void OnStartLoad();
		void OnStopLoad(Exception? exception);
		void OnCanceled();
	}
}
