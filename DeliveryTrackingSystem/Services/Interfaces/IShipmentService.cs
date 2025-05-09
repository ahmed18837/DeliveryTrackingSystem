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

        Task UpdateStatusAsync(int id, string status);

        Task<ShipmentDto> GetByTrackingNumberAsync(string trackingNumber);
        Task<IEnumerable<ShipmentDto>> FilterShipmentsAsync(ShipmentFilterDto filter);
        Task<ShipmentStatisticsDto> GetShipmentStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<TopDestinationDto>> GetTopDestinationsAsync(int topN);
    }
}
