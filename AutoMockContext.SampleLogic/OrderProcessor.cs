namespace AutoMockContext.SampleLogic
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OrderProcessor
	{
		private readonly ICustomerProvider _customerProvider;
		private readonly IInventoryService _inventoryService;
		private readonly IOrderRepository _orderRepository;
		private readonly ILogger _logger;

		public OrderProcessor(ICustomerProvider customerProvider,
							  IInventoryService inventoryService,
							  IOrderRepository orderRepository,
							  ILogger logger)
		{
			this._customerProvider = customerProvider;
			this._inventoryService = inventoryService;
			this._orderRepository = orderRepository;
			this._logger = logger;
		}

		public void CreateNewOrder(List<OrderItem> orderItems, Customer customer)
		{
			this._logger.Info($"Starting {nameof(this.CreateNewOrder)}");
			try
			{
				var order = this._orderRepository.SaveNewOrder(orderItems, customer);

				var inventorySession = this._inventoryService.OpenSession();
				if(orderItems.All(oi => this._inventoryService.TryReserveProduct(oi.ProductId, oi.Quantity)))
				{
					this._inventoryService.CommitSession(inventorySession);
					this._customerProvider.NotifyCustomerOfSuccessfulOrder(customer.CustomerId, order.OrderId);
				}
				else
				{
					this._inventoryService.RollbackSession(inventorySession);
					this._customerProvider.NotifyCustomerOfFailedOrder(customer.CustomerId, order.OrderId);
				}

				this._logger.Info($"Completed {nameof(this.CreateNewOrder)}");
			}
			catch(Exception ex)
			{
				this._logger.Error($"An error occurred in {nameof(this.CreateNewOrder)}: ${ex.Message}", ex);
			}

		}

		public void ReturnOrderItem(OrderItem orderItem, Customer customer)
		{
			this._logger.Info($"Starting {nameof(this.ReturnOrderItem)}");
			try
			{
				this._inventoryService.ReturnProduct(orderItem.ProductId, orderItem.Quantity);
				this._customerProvider.NotifyCustomerOfReturnedProduct(customer.CustomerId, orderItem.ProductId, orderItem.Quantity);
				this._logger.Info($"Completed {nameof(this.ReturnOrderItem)}");
			}
			catch (Exception ex)
			{
				this._logger.Error($"An error occurred in {nameof(this.ReturnOrderItem)}: ${ex.Message}", ex);
			}
		}
	}
}