namespace AutoMockHelper.xUnit
{
	using AutoMockHelper.Core;
	using AutoMockHelper.SampleLogic.OrderProcessor;
	using Xunit;

	public class OrderProcessorTests : AutoMockContext<OrderProcessor>
	{
		public OrderProcessorTests()
		{
			base.Setup();
		}

		[Fact]
		public void OrderProcessorIsSuccessfullyCreated()
		{
			//Arrange
			//Act
			var instance = this.ClassUnderTest;

			//Assert
			Assert.IsType<OrderProcessor>(instance);
		}
	}
}
