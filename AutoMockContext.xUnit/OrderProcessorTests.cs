namespace AutoMockContext.xUnit
{
    using AutoMockContext.SampleLogic.OrderProcessor;
    using AutoMockContext.Unity;
    using Xunit;

    public class OrderProcessorTests : AutoMockContextUnity<OrderProcessor>
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
