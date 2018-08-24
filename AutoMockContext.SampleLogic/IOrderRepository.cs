namespace AutoMockContext.SampleLogic
{
	using System.Collections.Generic;

	public interface IOrderRepository
	{
		Order SaveNewOrder(List<OrderItem> orderItems, Customer customer);
	}
}