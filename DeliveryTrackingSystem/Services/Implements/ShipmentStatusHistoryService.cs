using AutoMapper;
using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Interfaces;
using static DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory.ShipmentStatusHistoryCreateDto;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class ShipmentStatusHistoryService(IShipmentStatusHistoryRepository historyRepository, IMapper mapper) : IShipmentStatusHistoryService
    {
        private readonly IShipmentStatusHistoryRepository _historyRepository = historyRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ShipmentStatusHistoryDto>> GetAllAsync()
        {
            var histories = await _historyRepository.GetAllAsync();
            if (!histories.Any() || histories == null) throw new Exception("No history records found!");
            return _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(histories);
        }

        public async Task<ShipmentStatusHistoryDto> GetByIdAsync(int id)
        {
            var history = await _historyRepository.GetByIdAsync(id);
            return history != null ? _mapper.Map<ShipmentStatusHistoryDto>(history) : throw new Exception("History not found!");
        }

        public async Task CreateAsync(ShipmentStatusHistoryCreateDto dto)
        {
            var history = _mapper.Map<ShipmentStatusHistory>(dto);
            await _historyRepository.AddAsync(history);
        }

        public async Task UpdateAsync(int id, ShipmentStatusHistoryCreateDto dto)
        {
            var history = await _historyRepository.GetByIdAsync(id) ?? throw new Exception("History not found!");
            _mapper.Map(dto, history);
            await _historyRepository.UpdateAsync(id, history);
        }

        public async Task DeleteAsync(int id)
        {
            var history = await _historyRepository.GetByIdAsync(id) ?? throw new Exception("History not found!");
            await _historyRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ShipmentStatusHistoryDto>> GetStatusHistoryByShipmentIdAsync(int shipmentId)
        {
            var histories = await _historyRepository.GetStatusHistoryByShipmentIdAsync(shipmentId);
            if (!histories.Any()) throw new Exception("No status history found for this shipment!");
            return _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(histories);
        }

        public async Task<ShipmentStatusHistoryDto> GetLatestStatusChangeAsync(int shipmentId)
        {
            var history = await _historyRepository.GetLatestStatusChangeAsync(shipmentId);
            return history != null ? _mapper.Map<ShipmentStatusHistoryDto>(history) : throw new Exception("No status history found for this shipment!");
        }

        public async Task<IEnumerable<ShipmentStatusHistoryDto>> FilterStatusHistoryAsync(StatusHistoryFilterDto filter)
        {
            var statusHistoryFilter = _mapper.Map<StatusHistoryFilter>(filter);
            var histories = await _historyRepository.FilterStatusHistoryAsync(statusHistoryFilter);
            if (!histories.Any()) throw new Exception("No status history records found matching the criteria!");
            return _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(histories);
        }

        public async Task<StatusChangeStatisticsDto> GetStatusChangeStatisticsAsync(int? shipmentId, DateTime? startDate, DateTime? endDate)
        {
            var stats = await _historyRepository.GetStatusChangeStatisticsAsync(shipmentId, startDate, endDate);
            return _mapper.Map<StatusChangeStatisticsDto>(stats);
        }
    }
}