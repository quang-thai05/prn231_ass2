using AutoMapper;
using BussinessObject;
using Lab2.DTOs;
using Lab2.DTOs.User;

namespace Lab2.MapperProfile;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<OrderDetailDto, OrderDetail>().ReverseMap();
    }
}