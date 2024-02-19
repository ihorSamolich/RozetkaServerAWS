using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRozetka.Data.Entities.Addres;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Data.Entities.Photo;
using WebRozetka.Data.Entities.Product;

namespace WebRozetka.Data
{
    public class AppEFContext : IdentityDbContext<UserEntity, RoleEntity, int,
        IdentityUserClaim<int>, UserRoleEntity, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppEFContext(DbContextOptions<AppEFContext> options)
            : base(options)
        { }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<PhotoEntity> Photos { get; set; }

        public DbSet<BasketEntity> Baskets { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemsEntity> OrdersItems { get; set; }
        public DbSet<OrderStatusEntity> OrderStatuses { get; set; }
        public DbSet<OrderContactInfoEntity> OrderContactInfos { get; set; }

        public DbSet<AreaEntity> Areas { get; set; }
        public DbSet<SettlementEntity> Settlements { get; set; }
        public DbSet<WarehouseEntity> Warehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRoleEntity>(ur =>
            {
                ur.HasKey(ur => new { ur.UserId, ur.RoleId });
                ur.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(r => r.RoleId)
                    .IsRequired();
                ur.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired();
            });

            builder.Entity<BasketEntity>(ur =>
            {
                ur.HasKey(ur => new { ur.UserId, ur.ProductId });
            });
        }
    }
}
