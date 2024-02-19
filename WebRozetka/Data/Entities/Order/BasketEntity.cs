using System.ComponentModel.DataAnnotations.Schema;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data.Entities.Product;

namespace WebRozetka.Data.Entities.Order
{
    [Table("Basket")]
    public class BasketEntity
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual UserEntity User { get; set; }
        public virtual ProductEntity Product { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
