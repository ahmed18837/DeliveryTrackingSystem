using DeliveryTrackingSystem.Models.Dtos.Shipment;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IShipmentService
    {
        Task<IEnumerable<ShipmentDto>> GetAllAsync();
        Task<ShipmentDto> GetByIdAsync(int id);
        Task CreateAsync(ShipmentCreateDto dto);
        Task UpdateAsync(int id, ShipmentUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
