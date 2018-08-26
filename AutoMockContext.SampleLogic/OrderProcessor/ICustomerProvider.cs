namespace AutoMockContext.SampleLogic.OrderProcessor
{
	public interface ICustomerProvider
	{
		Customer GetCustomerDetails(int customerId);

		void NotifyCustomerOfSuccessfulOrder(int customerId, int orderId);
		void NotifyCustomerOfFailedOrder(int customerId, int orderId);
		void NotifyCustomerOfReturnedProduct(int customerId, int productId, int quantity);
	}
}