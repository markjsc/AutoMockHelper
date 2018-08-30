namespace AutoMockHelper.Samples.Logic.OrderProcessor
{
	public interface INotificationService
    {
        void NotifyCustomerOfSuccessfulOrder(int customerId, int orderNumber);
        void NotifyCustomerOfFailedOrder(int customerId, int orderNumber);
        void NotifyCustomerOfReturnedProduct(int customerId, int productId, int quantity);
    }
}