namespace AutoMockHelper.Samples.NUnit
{
	using AutoMockHelper.Core;
	using AutoMockHelper.SampleLogic.OrderProcessor;
	using global::NUnit.Framework;

	[TestFixture]
	public class OrderProcessorTests : AutoMockContext<OrderProcessor>
	{
		[SetUp]
		public void SetUp()
		{
			base.Setup();
		}

		[Test]
		public void OrderProcessorIsSuccessfullyCreated()
		{
			//Arrange
			//Act
			var instance = this.ClassUnderTest;

			//Assert
			Assert.IsInstanceOf(typeof(OrderProcessor), instance);
		}
	}
}