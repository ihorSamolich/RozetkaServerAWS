namespace WebRozetka.Models.Order
{

    public class OrderItemDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderViewModel
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string PostAddress { get; set; }
        public string OrderStatus { get; set; }
        public List<string> Products { get; set; }

    }
}


//public int OrderId { get; set; }
//public string CustomerName { get; set; }
//public string CustomerEmail { get; set; }
//public string CustomerPhone { get; set; }
//public List<OrderItemDetailViewModel> OrderItems { get; set; }