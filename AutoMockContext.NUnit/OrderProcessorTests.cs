namespace AutoMockContext.NUnit
{
	using AutoMockContext.Core;
	using AutoMockContext.SampleLogic;
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