using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebRozetka.Data.Entities.Product;

namespace WebRozetka.Data.Entities.Category
{
    [Table("Categories")]
    public class CategoryEntity : BaseEntity<int>
    {
        [Required, StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Image { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        public virtual ICollection<ProductEntity> Products { get; set; }
    }
}
