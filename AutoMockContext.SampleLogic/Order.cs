﻿namespace AutoMockContext.SampleLogic
{
	using System.Collections.Generic;

	public class Order
	{
		public int OrderId { get; set; }
		public List<OrderItem> OrderItems { get; set; }

		public Customer Customer { get; set; }
	}
}