namespace AutoMockHelper.SampleLogic.OrderProcessor
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private readonly List<Order> _orders = new List<Order>();

        public async Task<Customer> GetCustomerDetailsAsync(int customerId)
        {
            return await Task.Run(() => this._customers.FirstOrDefault(c => c.CustomerId == customerId));
        }

        public async Task<Order> SaveNewOrderAsync(int orderNumber, List<OrderItem> orderItems, Customer customer)
        {
            var newOrder = new Order
                           {
                               OrderNumber = orderNumber,
                               Customer = customer,
                               OrderItems = orderItems
                           };
            await Task.Run(() => this._orders.Add(newOrder));
            return newOrder;
        }

        public void AddNewCustomer(Customer customer)
        {
            this._customers.Add(customer);
        }

        public List<Order> GetOrders()
        {
            return this._orders;
        }
    }
}