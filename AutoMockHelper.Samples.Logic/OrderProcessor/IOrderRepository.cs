namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IOrderRepository
	{
		Task<Order> SaveNewOrderAsync(int orderNumber, List<OrderItem> orderItems, Customer customer);
	}
}