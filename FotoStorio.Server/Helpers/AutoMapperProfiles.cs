using AutoMapper;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name));

            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>().ReverseMap();

            CreateMap<Address, AddressDTO>();

            CreateMap<Order, OrderDTO>();
            CreateMap<Order, OrderDetailsDTO>();
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.ProductItemId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.ProductName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ItemOrdered.ImageUrl));
        }
    }
}