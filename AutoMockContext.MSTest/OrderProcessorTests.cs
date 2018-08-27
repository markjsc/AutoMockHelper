namespace AutoMockHelper.MSTest
{
	using System;
	using System.Collections.Generic;
	using AutoMockHelper.Core;
	using AutoMockHelper.SampleLogic.OrderProcessor;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Moq;

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

		[TestMethod]
		public void CreateNewOrderLogsException()
		{
			//Arrange
			this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
			this.MockFor<ILogger>().Setup(x => x.Error(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}")), It.IsAny<Exception>()));

			this.MockFor<IOrderRepository>().Setup(x => x.SaveNewOrder(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>()))
											.Throws(new ApplicationException("Order Repository is broken!"));

			//Act
			this.ClassUnderTest.CreateNewOrder(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>());

			//Assert
			this.VerifyAll();
		}
	}
}