using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Data.Entities.Photo;

namespace WebRozetka.Data.Entities.Product
{
    [Table("Products")]
    public class ProductEntity : BaseEntity<int>
    {
        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public decimal Price { get; set; }

        [Required, StringLength(4000)]
        public string Description { get; set; }
        public string Country { get; set; }
        public string Manufacturer { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100%.")]
        public decimal Discount { get; set; }


        [Required]
        public int CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }

        public virtual ICollection<PhotoEntity> Photos { get; set; }
        public virtual ICollection<BasketEntity> Baskets { get; set; }
        public virtual ICollection<OrderItemsEntity> OrderItems { get; set; }

    }
}
