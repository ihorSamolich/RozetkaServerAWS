using Microsoft.AspNetCore.Mvc;

namespace WebRozetka.Models.Product
{
    public class ProductPhotos
    {
        public string Photo { get; set; }
        public int Priority { get; set; }

    }

    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Manufacturer { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public int CategoryId { get; set; }

        public List<ProductPhotos> OldPhotos { get; set; }

        public List<ProductPhotos> NewPhotos { get; set; }

        //public List<string> oldPhotos { get; set; }

        //[BindProperty(Name = "newPhotos[]")]
        //public List<IFormFile> newPhotos { get; set; }

    }
}
