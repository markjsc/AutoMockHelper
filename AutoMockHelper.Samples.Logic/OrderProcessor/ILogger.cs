namespace AutoMockHelper.Samples.Logic.OrderProcessor
{
	using System;

	public interface ILogger
	{
		void Info(string message);
		void Error(string message, Exception exception);
	}
}