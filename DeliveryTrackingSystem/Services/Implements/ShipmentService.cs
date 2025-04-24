using AutoMapper;
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
    }
}
