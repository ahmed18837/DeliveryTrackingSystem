using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IShipmentStatusHistoryService
    {
        Task<IEnumerable<ShipmentStatusHistoryDto>> GetAllAsync();
        Task<ShipmentStatusHistoryDto> GetByIdAsync(int id);
        Task CreateAsync(ShipmentStatusHistoryCreateDto dto);
        Task UpdateAsync(int id, ShipmentStatusHistoryCreateDto dto);
        Task DeleteAsync(int id);
    }
}
