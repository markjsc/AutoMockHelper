namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Collections.Generic;

	public interface IOrderRepository
	{
		Order SaveNewOrder(List<OrderItem> orderItems, Customer customer);
	}
}