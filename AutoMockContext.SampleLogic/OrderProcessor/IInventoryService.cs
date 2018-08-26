namespace AutoMockContext.SampleLogic.OrderProcessor
{
	using System;

	public interface IInventoryService
	{
		Guid OpenSession();

		bool TryReserveProduct(int productId, int quantity);

		void CommitSession(Guid session);
		void RollbackSession(Guid session);

		void ReturnProduct(int productId, int quantity);
	}
}