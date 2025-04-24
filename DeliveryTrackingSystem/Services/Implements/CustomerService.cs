using AutoMapper;
using DeliveryTrackingSystem.Models.Dtos.Customer;
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
    }
}