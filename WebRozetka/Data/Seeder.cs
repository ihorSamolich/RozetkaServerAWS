using Bogus;
using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebRozetka.Constants;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Data.Entities.Photo;
using WebRozetka.Data.Entities.Product;
using WebRozetka.Helpers;
using WebRozetka.Interfaces;

namespace WebRozetka.Data
{
    public static class SeederDB
    {
        public static async void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = scope.ServiceProvider;

                var context = service.GetRequiredService<AppEFContext>();

                var userManager = service.GetRequiredService<UserManager<UserEntity>>();
                var roleManager = service.GetRequiredService<RoleManager<RoleEntity>>();
                var novaPoshta = service.GetRequiredService<INovaPoshtaService>();


                context.Database.Migrate();

                if (!context.Roles.Any())
                {
                    var admin = new RoleEntity
                    {
                        Name = Roles.Admin,
                    };
                    var roleResult = roleManager.CreateAsync(admin).Result;

                    var user = new RoleEntity
                    {
                        Name = Roles.User,
                    };

                    roleResult = roleManager.CreateAsync(user).Result;
                }

                if (!context.Users.Any())
                {
                    UserEntity user = new UserEntity
                    {
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "admin@gmail.com",
                        UserName = "admin@gmail.com",
                        Image = "admin.webp",
                        EmailConfirmed = true,
                    };
                    var result = userManager.CreateAsync(user, "123456").Result;
                    if (!result.Succeeded)
                    {
                        Console.WriteLine("-Error User Create");
                    }
                    else
                    {
                        result = userManager.AddToRoleAsync(user, Roles.Admin).Result;
                        if (!result.Succeeded)
                        {
                            Console.WriteLine("-Error User AddToRole");
                        }
                    }
                }

                if (!context.OrderStatuses.Any())
                {
                    var orderStatus = new List<OrderStatusEntity>
                    {
                        new OrderStatusEntity { Name = "В очікуванні" },
                        new OrderStatusEntity { Name = "Обробляється" },
                        new OrderStatusEntity { Name = "Відправлено" },
                        new OrderStatusEntity { Name = "Доставлено" },
                        new OrderStatusEntity { Name = "Завершено" },
                    };

                    context.OrderStatuses.AddRange(orderStatus);
                    context.SaveChanges();
                }

                if (!context.Areas.Any())
                {
                    novaPoshta.GetAreas();
                }

                if (!context.Settlements.Any())
                {
                    novaPoshta.GetSettlements();
                }

                if (!context.Warehouses.Any())
                {
                    novaPoshta.GetWarehouses();
                }

                if (!context.Categories.Any())
                {
                    Faker faker = new Faker();

                    var fakeCategory = new Faker<CategoryEntity>("uk")
                        .RuleFor(o => o.IsDeleted, f => false)
                        .RuleFor(o => o.DateCreated, f => DateTime.UtcNow)
                        .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                        .RuleFor(c => c.Description, f => f.Lorem.Paragraph());

                    var fakeCategories = fakeCategory.Generate(10);

                    foreach (var category in fakeCategories)
                    {
                        var fakeImage = await ImageWorker.SaveImageFromUrlAsync(faker.Image.LoremFlickrUrl());
                        category.Image = fakeImage;
                    }

                    context.Categories.AddRange(fakeCategories);
                    context.SaveChanges();
                }

                if (context.Products.Count() < 1000)
                {
                    Faker faker = new Faker();

                    var categoriesId = context.Categories.Where(c => !c.IsDeleted).Select(c => c.Id).ToList();

                    var fakeProduct = new Faker<ProductEntity>("uk")
                         .RuleFor(o => o.IsDeleted, f => false)
                         .RuleFor(o => o.DateCreated, f => DateTime.UtcNow)
                         .RuleFor(o => o.Name, f => f.Commerce.ProductName())
                         .RuleFor(o => o.Price, f => Math.Round(f.Random.Decimal(1, 1000), 2))
                         .RuleFor(o => o.Description, f => f.Lorem.Paragraph())
                         .RuleFor(o => o.Country, f => f.Address.Country())
                         .RuleFor(o => o.Manufacturer, f => f.Company.CompanyName())
                         .RuleFor(o => o.Quantity, f => f.Random.Number(0, 1000))
                         .RuleFor(o => o.Discount, f => f.Random.Number(0, 99))
                         .RuleFor(o => o.CategoryId, f => f.PickRandom(categoriesId));

                    var fakeProducts = fakeProduct.Generate(200);

                    context.Products.AddRange(fakeProducts);
                    context.SaveChanges();

                    var photos = new List<PhotoEntity>();

                    foreach (var product in fakeProducts)
                    {
                        var numberOfPhotos = faker.Random.Number(1, 3);

                        for (int i = 0; i < numberOfPhotos; i++)
                        {
                            var fakeImage = await ImageWorker.SaveImageFromUrlAsync(faker.Image.LoremFlickrUrl());
                            photos.Add(new PhotoEntity { FilePath = fakeImage, ProductId = product.Id });
                        }
                    }
                    context.AddRange(photos);
                    context.SaveChanges();
                }
            }
        }
    }
}
