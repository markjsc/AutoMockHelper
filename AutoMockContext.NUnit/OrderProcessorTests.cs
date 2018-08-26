namespace AutoMockContext.NUnit
{
    using AutoMockContext.SampleLogic.OrderProcessor;
    using AutoMockContext.Unity;
    using global::NUnit.Framework;

    [TestFixture]
	public class OrderProcessorTests : AutoMockContextUnity<OrderProcessor>
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