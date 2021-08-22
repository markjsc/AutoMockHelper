namespace AutoMockHelper.Samples.Logic.OrderProcessor
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage] //No need to test a simple model class.
    public class Order
    {
        public int OrderNumber { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Customer Customer { get; set; }
    }
}