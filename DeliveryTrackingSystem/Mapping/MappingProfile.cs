using AutoMapper;
using DeliveryTrackingSystem.Models.Dtos.Auth;
using DeliveryTrackingSystem.Models.Dtos.Customer;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;
using DeliveryTrackingSystem.Models.Dtos.User;
using DeliveryTrackingSystem.Models.Entities;

namespace DeliveryTrackingSystem.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mapping
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, RegisterDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();

            CreateMap<RequestRegisterDto, User>().ReverseMap();

            // Customer Mapping
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();

            // Shipment Mapping
            CreateMap<Shipment, ShipmentDto>().ReverseMap();
            CreateMap<Shipment, ShipmentCreateDto>().ReverseMap();
            CreateMap<Shipment, ShipmentUpdateDto>().ReverseMap();

            // ShipmentStatusHistory Mapping
            CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryDto>().ReverseMap();
            CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryCreateDto>().ReverseMap();
            CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryUpdateDto>().ReverseMap();
        }
    }
}
