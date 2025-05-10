using DeliveryTrackingSystem.Models.Dtos.Customer;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingSystem.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;

        [Authorize(Roles = "SuperAdmin, Admin, Employee")]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var customers = await _customerService.GetAllAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin, Employee")]
        [HttpGet("{id:int}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _customerService.CreateAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CustomerUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _customerService.UpdateAsync(id, dto);
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _customerService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut("{id}/email")]
        public async Task<IActionResult> UpdateEmail(int id, [FromBody] string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _customerService.UpdateEmailAsync(id, email);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("{customerId:int}/shipments")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetCustomerShipments(int customerId)
        {
            try
            {
                var shipments = await _customerService.GetCustomerShipmentsAsync(customerId);
                return Ok(shipments);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("shipments/{shipmentId:int}/status-history")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetShipmentStatusHistory(int shipmentId)
        {
            try
            {
                var history = await _customerService.GetShipmentStatusHistoryAsync(shipmentId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("{customerId:int}/shipment-summary")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetCustomerShipmentSummary(int customerId)
        {
            try
            {
                var summary = await _customerService.GetCustomerShipmentSummaryAsync(customerId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest("Search term is required.");
            try
            {
                var customers = await _customerService.SearchCustomersAsync(searchTerm);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("filter-by-shipments")]
        public async Task<IActionResult> FilterCustomersByShipmentCount([FromQuery] int minShipments, [FromQuery] int maxShipments)
        {
            if (minShipments < 0 || maxShipments < minShipments) return BadRequest("Invalid shipment count range.");
            try
            {
                var customers = await _customerService.FilterCustomersByShipmentCountAsync(minShipments, maxShipments);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }

}
