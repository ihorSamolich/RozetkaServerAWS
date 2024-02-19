namespace WebRozetka.Models.Order
{

    public class OrderItemDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderItemDetailViewModel> OrderItems { get; set; }

    }
}
