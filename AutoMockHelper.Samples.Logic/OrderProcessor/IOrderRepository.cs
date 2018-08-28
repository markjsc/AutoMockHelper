﻿namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IOrderRepository
	{
	    Task<Customer> GetCustomerDetailsAsync(int customerId);

		Task<Order> SaveNewOrderAsync(List<OrderItem> orderItems, Customer customer);
	}
}