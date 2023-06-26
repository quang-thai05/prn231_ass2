using AutoMapper;
using BussinessObject;
using Lab2.DTOs;

namespace Lab2.MapperProfile;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, OrderDetailDto>().ReverseMap();
    }
}