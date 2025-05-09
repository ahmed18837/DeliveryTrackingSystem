using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentController(IShipmentService shipmentService) : ControllerBase
    {
        private readonly IShipmentService _shipmentService = shipmentService;

        [Authorize()]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var shipments = await _shipmentService.GetAllAsync();
                return Ok(shipments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("{id:int}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var shipment = await _shipmentService.GetByIdAsync(id);
                return Ok(shipment);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShipmentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _shipmentService.CreateAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShipmentUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _shipmentService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shipmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpPut("{shipmentId}/status")]
        public async Task<IActionResult> UpdateShipmentStatus(int shipmentId, [FromBody] string status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _shipmentService.UpdateStatusAsync(shipmentId, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("track/{trackingNumber}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetByTrackingNumber(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) return BadRequest("Tracking number is required.");
            try
            {
                var shipment = await _shipmentService.GetByTrackingNumberAsync(trackingNumber);
                return Ok(shipment);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("Filter")]
        public async Task<IActionResult> FilterShipments([FromQuery] ShipmentFilterDto filter)
        {
            try
            {
                var shipments = await _shipmentService.FilterShipmentsAsync(filter);
                return Ok(shipments);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetShipmentStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var statistics = await _shipmentService.GetShipmentStatisticsAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("top-destinations")]
        public async Task<IActionResult> GetTopDestinations([FromQuery] int topN = 5)
        {
            if (topN <= 0) return BadRequest("TopN must be greater than 0.");
            try
            {
                var destinations = await _shipmentService.GetTopDestinationsAsync(topN);
                return Ok(destinations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
