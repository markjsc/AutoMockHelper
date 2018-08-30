namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Diagnostics.CodeAnalysis;

	[ExcludeFromCodeCoverage] //No need to test a simple model class.
	public class OrderItem
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}