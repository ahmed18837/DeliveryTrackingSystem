using DeliveryTrackingSystem.Models.Dtos.Auth;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task AdminRegisterAsync(RequestRegisterDto request);
        Task EmployeeRegisterAsync(RequestRegisterDto request);
        Task DriverRegisterAsync(RequestRegisterDto request);
        Task<ResponseDto> LoginAsync(LoginDto request);

    }
}
