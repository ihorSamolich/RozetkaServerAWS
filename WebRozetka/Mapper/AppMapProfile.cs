using AutoMapper;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Addres;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Identity;
using WebRozetka.Data.Entities.Order;
using WebRozetka.Data.Entities.Photo;
using WebRozetka.Data.Entities.Product;
using WebRozetka.Helpers;
using WebRozetka.Mapper.Converters;
using WebRozetka.Models.Account;
using WebRozetka.Models.Address;
using WebRozetka.Models.Basket;
using WebRozetka.Models.Category;
using WebRozetka.Models.Order;
using WebRozetka.Models.Product;

namespace WebRozetka.Mapper
{
    public class AppMapProfile : Profile
    {
        private readonly AppEFContext _context;
        public AppMapProfile(AppEFContext context)
        {
            _context = context;

            CreateMap<CategoryCreateViewModel, CategoryEntity>()
                .AfterMap((src, dest) =>
                {
                    dest.IsDeleted = false;
                    dest.DateCreated = DateTime.UtcNow;
                    dest.Image = ImageWorker.SaveImageAsync(src.Image).Result;
                });

            CreateMap<CategoryEntity, CategoryItemViewModel>();

            CreateMap<ICollection<PhotoEntity>, List<string>>()
                .ConvertUsing<PhotoEntityToFilePathConverter>();

            CreateMap<ProductEntity, ProductViewModel>()
               .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));

            CreateMap<ProductViewModel, ProductEntity>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            CreateMap<ProductCreateViewModel, ProductEntity>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.IsDeleted = false;
                    dest.DateCreated = DateTime.UtcNow;
                });

            CreateMap<ProductEditViewModel, ProductEntity>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore());

            CreateMap<NPAreaItemViewModel, AreaEntity>();

            CreateMap<AreaEntity, AreaViewModel>();

            CreateMap<NPSettlementItemViewModel, SettlementEntity>()
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => _context.Areas.Where(x => x.Ref == src.Area).Select(x => x.Id).SingleOrDefault()))
                .ForMember(dest => dest.Area, opt => opt.Ignore());
            CreateMap<SettlementEntity, SettlementViewModel>();

            CreateMap<NPWarehouseItemViewModel, WarehouseEntity>()
              .ForMember(dest => dest.SettlementId, opt => opt.MapFrom(src => _context.Settlements.Where(x => x.Ref == src.SettlementRef).Select(x => x.Id).SingleOrDefault()))
              .ForMember(dest => dest.Settlement, opt => opt.Ignore());

            CreateMap<WarehouseEntity, WarehouseViewModel>();

            CreateMap<WarehouseEntity, WarehouseFullViewModel>();

            CreateMap<BasketEntity, BasketItemViewModel>()
               .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.Product.Name))
               .ForMember(x => x.Price, opt => opt.MapFrom(x => x.Product.Price))
               .ForMember(x => x.Quantity, opt => opt.MapFrom(x => x.Product.Quantity))
               .ForMember(x => x.Photos, opt => opt.MapFrom(x => x.Product.Photos));
        }
    }



}
