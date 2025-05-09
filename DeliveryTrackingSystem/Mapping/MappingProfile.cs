using AutoMapper;
using DeliveryTrackingSystem.Helper;
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
            // User <-> UserDto
            CreateMap<User, UserDto>()
    .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => new List<string> { src.Role }));

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Roles != null && src.Roles.Any() ? src.Roles.First() : null))
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore()); // Ignore navigation property

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

            // CustomerShipmentSummaryDto Mapping
            CreateMap<(int TotalShipments, decimal TotalCost, Dictionary<ShipmentStatus, int> StatusBreakdown), CustomerShipmentSummaryDto>()
                .ForMember(dest => dest.TotalShipments, opt => opt.MapFrom(src => src.TotalShipments))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost))
                .ForMember(dest => dest.StatusBreakdown, opt => opt.MapFrom(src => src.StatusBreakdown));

            // ActiveCustomerDto Mapping
            CreateMap<(Customer Customer, int ShipmentCount), ActiveCustomerDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
                .ForMember(dest => dest.ShipmentCount, opt => opt.MapFrom(src => src.ShipmentCount));
            CreateMap<Customer, ActiveCustomerDto>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.ShipmentCount, opt => opt.MapFrom(src => src.Shipments != null ? src.Shipments.Count : 0));
        }
    }
}
