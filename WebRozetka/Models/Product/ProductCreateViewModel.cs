using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebRozetka.Data.Entities;

namespace WebRozetka.Models.Product
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Manufacturer { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public int CategoryId { get; set; }

        [BindProperty(Name = "images[]")]
        public List<IFormFile> Images { get; set; }
    }
}
