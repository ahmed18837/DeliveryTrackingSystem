using AutoMapper;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Interfaces;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class ShipmentService(IShipmentRepository shipmentRepository, IMapper mapper) : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository = shipmentRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<ShipmentDto>> GetAllAsync()
        {
            var shipments = await _shipmentRepository.GetAllAsync();
            if (!shipments.Any() || shipments == null) throw new Exception("No shipments found!");
            return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
        }

        public async Task<ShipmentDto> GetByIdAsync(int id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id);
            return shipment != null ? _mapper.Map<ShipmentDto>(shipment) : throw new Exception("Shipment not found!");
        }

        public async Task CreateAsync(ShipmentCreateDto dto)
        {
            var shipment = _mapper.Map<Shipment>(dto);
            await _shipmentRepository.AddAsync(shipment);
        }

        public async Task UpdateAsync(int id, ShipmentUpdateDto dto)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id) ?? throw new Exception("Shipment not found!");
            _mapper.Map(dto, shipment);
            await _shipmentRepository.UpdateAsync(id, shipment);
        }

        public async Task DeleteAsync(int id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id) ?? throw new Exception("Shipment not found!");
            await _shipmentRepository.DeleteAsync(id);
        }

        public async Task<ShipmentDto> GetByTrackingNumberAsync(string trackingNumber)
        {
            var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
            return shipment != null ? _mapper.Map<ShipmentDto>(shipment) : throw new Exception("Shipment not found!");
        }

        public async Task<IEnumerable<ShipmentDto>> FilterShipmentsAsync(ShipmentFilterDto filter)
        {
            var shipmentFilter = _mapper.Map<ShipmentFilterDto>(filter);
            var shipments = await _shipmentRepository.FilterShipmentsAsync(shipmentFilter);
            if (!shipments.Any()) throw new Exception("No shipments found matching the criteria!");
            return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
        }

        public async Task<ShipmentStatisticsDto> GetShipmentStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var stats = await _shipmentRepository.GetShipmentStatisticsAsync(startDate, endDate);
            return _mapper.Map<ShipmentStatisticsDto>(stats);
        }

        public async Task<IEnumerable<TopDestinationDto>> GetTopDestinationsAsync(int topN)
        {
            var destinations = await _shipmentRepository.GetTopDestinationsAsync(topN);
            return _mapper.Map<IEnumerable<TopDestinationDto>>(destinations);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id) ?? throw new Exception($"Shipment not found.");

            if (!Enum.TryParse<ShipmentStatus>(status, true, out var newStatus))
                throw new Exception("Invalid shipment status.");

            shipment.Status = newStatus;
            await _shipmentRepository.UpdateAsync(id, shipment);
        }
    }
}
