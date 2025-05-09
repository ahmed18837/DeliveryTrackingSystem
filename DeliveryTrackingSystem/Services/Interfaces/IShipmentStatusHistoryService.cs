using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;
using static DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory.ShipmentStatusHistoryCreateDto;

namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IShipmentStatusHistoryService
    {
        Task<IEnumerable<ShipmentStatusHistoryDto>> GetAllAsync();
        Task<ShipmentStatusHistoryDto> GetByIdAsync(int id);
        Task CreateAsync(ShipmentStatusHistoryCreateDto dto);
        Task UpdateAsync(int id, ShipmentStatusHistoryCreateDto dto);
        Task DeleteAsync(int id);

        Task<IEnumerable<ShipmentStatusHistoryDto>> GetStatusHistoryByShipmentIdAsync(int shipmentId);
        Task<ShipmentStatusHistoryDto> GetLatestStatusChangeAsync(int shipmentId);
        Task<IEnumerable<ShipmentStatusHistoryDto>> FilterStatusHistoryAsync(StatusHistoryFilterDto filter);
        Task<StatusChangeStatisticsDto> GetStatusChangeStatisticsAsync(int? shipmentId, DateTime? startDate, DateTime? endDate);
    }
}
