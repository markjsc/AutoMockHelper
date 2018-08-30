namespace AutoMockHelper.Samples.Logic.OrderProcessor
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IInventoryService
	{
		Task<Guid> OpenSessionAsync();

		Task<bool> TryReserveProductsAsync(List<OrderItem> orderItems);

		Task CommitSessionAsync(Guid session);
		Task RollbackSessionAsync(Guid session);

		Task ReturnProductAsync(int productId, int quantity);
	}
}