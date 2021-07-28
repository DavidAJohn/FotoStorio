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
        }
    }
}