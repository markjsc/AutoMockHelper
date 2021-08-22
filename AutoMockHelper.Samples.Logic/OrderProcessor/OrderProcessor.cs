﻿namespace AutoMockHelper.Samples.Logic.OrderProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class OrderProcessor
    {
        private readonly INotificationService _notificationService;
        private readonly IInventoryService _inventoryService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderNumberGeneratorService _orderNumberGeneratorService;

        private readonly ILogger _logger;

        public OrderProcessor(INotificationService notificationService,
                              IInventoryService inventoryService,
                              IOrderRepository orderRepository,
                              IOrderNumberGeneratorService orderNumberGeneratorService,
                              ILogger logger)
        {
            this._notificationService = notificationService;
            this._inventoryService = inventoryService;
            this._orderRepository = orderRepository;
            this._orderNumberGeneratorService = orderNumberGeneratorService;
            this._logger = logger;
        }

        public async Task CreateNewOrder(List<OrderItem> orderItems, Customer customer)
        {
            this._logger.Info($"Starting {nameof(this.CreateNewOrder)}");
            try
            {
                var orderNumber = this._orderNumberGeneratorService.GetNextOrderNumber();
                var order = await this._orderRepository.SaveNewOrderAsync(orderNumber, orderItems, customer);

                var inventorySessionId = await this._inventoryService.OpenSessionAsync();

                if (await this._inventoryService.TryReserveProductsAsync(orderItems))
                {
                    await this._inventoryService.CommitSessionAsync(inventorySessionId);
                    this._notificationService.NotifyCustomerOfSuccessfulOrder(customer.CustomerId, order.OrderNumber);
                }
                else
                {
                    await this._inventoryService.RollbackSessionAsync(inventorySessionId);
                    this._notificationService.NotifyCustomerOfFailedOrder(customer.CustomerId, order.OrderNumber);
                }

                this._logger.Info($"Completed {nameof(this.CreateNewOrder)}");
            }
            catch (Exception ex)
            {
                this._logger.Error($"An error occurred in {nameof(this.CreateNewOrder)}: {ex.Message}", ex);
            }

        }

        public async Task ReturnOrderItem(OrderItem orderItem, Customer customer)
        {
            this._logger.Info($"Starting {nameof(this.ReturnOrderItem)}");
            try
            {
                await this._inventoryService.ReturnProductAsync(orderItem.ProductId, orderItem.Quantity);
                this._notificationService.NotifyCustomerOfReturnedProduct(customer.CustomerId, orderItem.ProductId, orderItem.Quantity);
                this._logger.Info($"Completed {nameof(this.ReturnOrderItem)}");
            }
            catch (Exception ex)
            {
                this._logger.Error($"An error occurred in {nameof(this.ReturnOrderItem)}: {ex.Message}", ex);
            }
        }
    }
}