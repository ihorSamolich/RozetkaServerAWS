using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebRozetka.Data.Entities.Addres;
using WebRozetka.Data.Entities.Identity;

namespace WebRozetka.Data.Entities.Order
{
    [Table("Order")]
    public class OrderEntity : BaseEntity<int>
    {

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }

        [ForeignKey("OrderStatus")]
        public int OrderStatusId { get; set; }
        public virtual OrderStatusEntity OrderStatus { get; set; }

        public virtual ICollection<OrderItemsEntity> OrderItems { get; set; }
        public virtual OrderContactInfoEntity OrderContactInfo { get; set; }
    }
}
