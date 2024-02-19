using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebRozetka.Data.Entities.Product;

namespace WebRozetka.Data.Entities.Order
{
    [Table("OrderItems")]
    public class OrderItemsEntity : BaseEntity<int>
    {
        [Required]
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; }

        [Required]
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 0.")]
        public int Count { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }
    }
}
