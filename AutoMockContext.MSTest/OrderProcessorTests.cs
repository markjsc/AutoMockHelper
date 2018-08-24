namespace AutoMockContext.MSTest
{
	using AutoMockContext.Core;
	using AutoMockContext.SampleLogic;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class OrderProcessorTests : AutoMockContext<OrderProcessor>
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