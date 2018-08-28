namespace AutoMockHelper.Samples.MSTest
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
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
		public async Task CreateNewOrderLogsException()
		{
			//Arrange
		    this.StrictMock<ILogger>();
			this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
			this.MockFor<ILogger>().Setup(x => x.Error(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}")), It.IsAny<Exception>()));

		    this.StrictMock<IOrderRepository>();
		    this.MockFor<IOrderRepository>().Setup(x => x.SaveNewOrderAsync(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>()))
		        .Throws(new ApplicationException("Order Repository is broken!"));

			//Act
			await this.ClassUnderTest.CreateNewOrder(It.IsAny<List<OrderItem>>(), It.IsAny<Customer>());

			//Assert
			this.VerifyAll();
		}

		[TestMethod]
		public async Task CreateNewOrderRollsBackAndNotifiesWhenTryReserveProductFails()
		{
			//Arrange
		    const bool ReserveProductIsSuccessful = false;
		    var testSessionId = Guid.NewGuid();
		    var testCustomer = new Customer
		                   {
		                       CustomerId = 42
		                   };
		    var testOrder = new Order
		                {
		                    Customer = testCustomer,
		                    OrderId = 999
		                };

		    this.StrictMock<ILogger>();
			this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
			this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.CreateNewOrder)}"))));

		    this.StrictMock<IOrderRepository>();
		    this.MockFor<IOrderRepository>().Setup(x => x.SaveNewOrderAsync(It.IsAny<List<OrderItem>>(), It.Is<Customer>(c => c.CustomerId == testCustomer.CustomerId)))
		        .ReturnsAsync(testOrder);

		    this.StrictMock<IInventoryService>();
		    this.MockFor<IInventoryService>().Setup(x => x.OpenSessionAsync())
		        .ReturnsAsync(testSessionId);
			this.MockFor<IInventoryService>().Setup(x => x.TryReserveProductsAsync(It.IsAny<List<OrderItem>>()))
			    .ReturnsAsync(ReserveProductIsSuccessful);
		    this.MockFor<IInventoryService>().Setup(x => x.RollbackSessionAsync(testSessionId))
		        .Returns(Task.CompletedTask);

		    this.StrictMock<INotificationService>();
		    this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfFailedOrder(testCustomer.CustomerId, testOrder.OrderId));

		    //Act
		    await this.ClassUnderTest.CreateNewOrder(new List<OrderItem>(), testCustomer);

		    //Assert
		    this.VerifyAll();
		}

	    [TestMethod]
	    public async Task CreateNewOrderCompletesSuccessfully()
	    {
	        //Arrange
	        const bool ReserveProductIsSuccessful = true;
	        var testSessionId = Guid.NewGuid();
	        var testCustomer = new Customer
	                           {
	                               CustomerId = 42
	                           };
	        var testOrder = new Order
	                        {
	                            Customer = testCustomer,
	                            OrderId = 999
	                        };

	        this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"{nameof(OrderProcessor.CreateNewOrder)}"))));
	        this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.CreateNewOrder)}"))));

	        this.MockFor<IOrderRepository>().Setup(x => x.SaveNewOrderAsync(It.IsAny<List<OrderItem>>(), It.Is<Customer>(c => c.CustomerId == testCustomer.CustomerId)))
	            .ReturnsAsync(testOrder);

	        this.MockFor<IInventoryService>().Setup(x => x.OpenSessionAsync())
	            .ReturnsAsync(testSessionId);
	        this.MockFor<IInventoryService>().Setup(x => x.TryReserveProductsAsync(It.IsAny<List<OrderItem>>()))
	            .ReturnsAsync(ReserveProductIsSuccessful);
	        this.MockFor<IInventoryService>().Setup(x => x.CommitSessionAsync(testSessionId))
	            .Returns(Task.CompletedTask);

	        this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfSuccessfulOrder(testCustomer.CustomerId, testOrder.OrderId));

	        //Act
	        await this.ClassUnderTest.CreateNewOrder(new List<OrderItem>(), testCustomer);

	        //Assert
	        this.VerifyCallsFor<IInventoryService>();
	        this.VerifyCallsFor<INotificationService>();
	    }

	    [TestMethod]
	    public async Task ReturnOrderItemLogsFailure()
	    {
	        //Arrange
	        var testOrderItem = new OrderItem
	                            {
	                                Quantity = 42,
	                                ProductId = 999
	                            };
	        var testCustomer = new Customer();

	        this.MockFor<IInventoryService>().Setup(x => x.ReturnProductAsync(testOrderItem.ProductId, testOrderItem.Quantity))
	            .ThrowsAsync(new ApplicationException("Returning Product has failed!"));

	        this.MockFor<ILogger>().Setup(x => x.Error(It.Is<string>(m => m.Contains("error")), It.IsAny<ApplicationException>()));

	        //Act
	        await this.ClassUnderTest.ReturnOrderItem(testOrderItem, testCustomer);

	        //Assert
	        this.VerifyCallsFor<ILogger>();
	    }

	    [TestMethod]
	    public async Task ReturnOrderItemCompletesSuccessfully()
	    {
	        //Arrange
	        var testOrderItem = new OrderItem
	                            {
	                                Quantity = 42,
	                                ProductId = 999
	                            };
	        var testCustomer = new Customer
	                           {
                                   CustomerId = 768
	                           };

	        this.MockFor<IInventoryService>().Setup(x => x.ReturnProductAsync(testOrderItem.ProductId, testOrderItem.Quantity))
	            .Returns(Task.CompletedTask);

	        this.MockFor<INotificationService>().Setup(x => x.NotifyCustomerOfReturnedProduct(testCustomer.CustomerId, testOrderItem.ProductId, testOrderItem.Quantity));

	        this.MockFor<ILogger>().Setup(x => x.Info(It.Is<string>(m => m.Contains($"Completed {nameof(OrderProcessor.ReturnOrderItem)}"))));

	        //Act
	        await this.ClassUnderTest.ReturnOrderItem(testOrderItem, testCustomer);

	        //Assert
	        this.VerifyAll();
	    }
	}
}