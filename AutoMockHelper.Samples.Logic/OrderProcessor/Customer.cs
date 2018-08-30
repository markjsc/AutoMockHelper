namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Diagnostics.CodeAnalysis;

	[ExcludeFromCodeCoverage] //No need to test a simple model class.
	public class Customer
	{
		public int CustomerId { get; set; }
		public string FullName { get; set; }
		public string EmailAddress { get; set; }
		public string FormattedShippingAddress { get; set; }
	}
}