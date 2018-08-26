namespace AutoMockContext.MSTest
{
    using AutoMockContext.SampleLogic.OrderProcessor;
    using AutoMockContext.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
	public class OrderProcessorTests : AutoMockContextUnity<OrderProcessor>
	{
		[TestInitialize]
		public void Initialize()
		{
			base.Setup();
		}

		[TestMethod]
		public void OrderProcessorIsSuccessfullyCreated()
		{
			//Arrange
			//Act
			var instance = this.ClassUnderTest;

			//Assert
			Assert.IsInstanceOfType(instance, typeof(OrderProcessor));
		}
	}
}