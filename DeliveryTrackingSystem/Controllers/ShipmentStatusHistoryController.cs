using DeliveryTrackingSystem.Models.Dtos.ShipmentStatusHistory;
using DeliveryTrackingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentStatusHistoryController(IShipmentStatusHistoryService historyService) : ControllerBase
    {
        private readonly IShipmentStatusHistoryService _historyService = historyService;

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var history = await _historyService.GetAllAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var status = await _historyService.GetByIdAsync(id);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShipmentStatusHistoryCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _historyService.CreateAsync(dto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShipmentStatusHistoryCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _historyService.UpdateAsync(id, dto);
                return StatusCode(201);
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
                await _historyService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("shipment/{shipmentId:int}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetStatusHistoryByShipmentId(int shipmentId)
        {
            try
            {
                var histories = await _historyService.GetStatusHistoryByShipmentIdAsync(shipmentId);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("shipment/{shipmentId:int}/latest")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> GetLatestStatusChange(int shipmentId)
        {
            try
            {
                var history = await _historyService.GetLatestStatusChangeAsync(shipmentId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterStatusHistory([FromQuery] StatusHistoryFilterDto filter)
        {
            try
            {
                var histories = await _historyService.FilterStatusHistoryAsync(filter);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize("SuperAdmin, Admin, Employee")]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatusChangeStatistics([FromQuery] int? shipmentId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var statistics = await _historyService.GetStatusChangeStatisticsAsync(shipmentId, startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
