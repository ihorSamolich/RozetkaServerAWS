﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRozetka.Data.Entities.Order
{
    [Table("OrderStatus")]
    public class OrderStatusEntity : BaseEntity<int>
    {
        [Required, StringLength(255)]
        public string Name { get; set; }
        public virtual ICollection<OrderEntity> Orders { get; set; }
    }
}
