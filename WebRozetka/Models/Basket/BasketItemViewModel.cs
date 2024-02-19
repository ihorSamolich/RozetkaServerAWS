namespace WebRozetka.Models.Basket
{
    public class BasketItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public short Count { get; set; }
        public int Quantity { get; set; }
        public List<string> Photos { get; set; }
    }
}
