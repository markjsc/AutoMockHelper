using Xunit;

namespace AutoMockContext.xUnit
{
	using AutoMockContext.Core;
	using AutoMockContext.SampleLogic;

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
