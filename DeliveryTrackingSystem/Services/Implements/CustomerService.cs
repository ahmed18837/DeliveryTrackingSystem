using AutoMapper;
using DeliveryTrackingSystem.Models.Dtos.Customer;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.Interfaces;
using DeliveryTrackingSystem.Services.Interfaces;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class CustomerService(ICustomerRepository customerRepository, IMapper mapper) : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            if (!customers.Any() || customers == null) throw new Exception("No customers found!");
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer != null ? _mapper.Map<CustomerDto>(customer) : throw new Exception("Customer not found!");
        }

        public async Task CreateAsync(CustomerCreateDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _customerRepository.GetByIdAsync(id) ?? throw new Exception("Customer not found!");
            _mapper.Map(dto, customer);
            await _customerRepository.UpdateAsync(id, customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id) ?? throw new Exception("Customer not found!");
            await _customerRepository.DeleteAsync(id);
        }

        public async Task UpdateEmailAsync(int customerId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            if (email.Length > 100)
                throw new ArgumentException("Email exceeds maximum length of 100 characters", nameof(email));

            // Basic email format validation
            if (!email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Invalid email format");

            var customer = await _customerRepository.GetByIdAsync(customerId) ??
                throw new KeyNotFoundException($"Customer not found");

            customer.Email = email;
            await _customerRepository.UpdateAsync(customerId, customer);
        }

        public async Task<IEnumerable<ShipmentDto>> GetCustomerShipmentsAsync(int customerId)
        {
            var shipments = await _customerRepository.GetCustomerShipmentsAsync(customerId);
            if (!shipments.Any()) throw new Exception("No shipments found for this customer!");
            return _mapper.Map<IEnumerable<ShipmentDto>>(shipments);
        }

        public async Task<IEnumerable<ShipmentStatusHistoryDto>> GetShipmentStatusHistoryAsync(int shipmentId)
        {
            var history = await _customerRepository.GetShipmentStatusHistoryAsync(shipmentId);
            if (!history.Any()) throw new Exception("No status history found for this shipment!");
            return _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(history);
        }

        public async Task<CustomerShipmentSummaryDto> GetCustomerShipmentSummaryAsync(int customerId)
        {
            var (totalShipments, totalCost, statusBreakdown) = await _customerRepository.GetCustomerShipmentSummaryAsync(customerId);
            return new CustomerShipmentSummaryDto
            {
                TotalShipments = totalShipments,
                TotalCost = totalCost,
                StatusBreakdown = statusBreakdown
            };
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _customerRepository.SearchCustomersAsync(searchTerm);
            if (!customers.Any()) throw new Exception("No customers found matching the search term!");
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<IEnumerable<CustomerDto>> FilterCustomersByShipmentCountAsync(int minShipments, int maxShipments)
        {
            var customers = await _customerRepository.FilterCustomersByShipmentCountAsync(minShipments, maxShipments);
            if (!customers.Any()) throw new Exception("No customers found with the specified shipment count range!");
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

    }
}