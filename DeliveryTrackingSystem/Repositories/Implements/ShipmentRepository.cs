using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Dtos.Shipment;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class ShipmentRepository(AppDbContext context) : GenericRepository<Shipment>(context), IShipmentRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<Shipment> GetByTrackingNumberAsync(string trackingNumber)
        {
            return await _context.Shipments
                .Include(s => s.Customer)
                .Include(s => s.Driver)
                .Include(s => s.StatusHistories)
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        }

        public async Task<IEnumerable<Shipment>> FilterShipmentsAsync(ShipmentFilterDto filter)
        {
            var query = _context.Shipments
                .Include(s => s.Customer)
                .Include(s => s.Driver)
                .AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(s => s.Status == filter.Status.Value);
            if (filter.CustomerId.HasValue)
                query = query.Where(s => s.CustomerId == filter.CustomerId.Value);
            if (filter.DriverId.HasValue)
                query = query.Where(s => s.DriverId == filter.DriverId.Value);
            if (filter.StartDate.HasValue)
                query = query.Where(s => s.CreatedAt >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(s => s.CreatedAt <= filter.EndDate.Value);

            return await query.ToListAsync();
        }

        public async Task<(int TotalShipments, decimal AverageCost, Dictionary<ShipmentStatus, int> StatusBreakdown)> GetShipmentStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Shipments.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.CreatedAt <= endDate.Value);

            var shipments = await query.ToListAsync();

            var totalShipments = shipments.Count;
            var averageCost = shipments.Any() ? shipments.Average(s => s.ShippingCost) : 0;
            var statusBreakdown = shipments
                .GroupBy(s => s.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            return (totalShipments, averageCost, statusBreakdown);
        }

        public async Task<IEnumerable<(string Destination, int ShipmentCount)>> GetTopDestinationsAsync(int topN)
        {
            var query = await _context.Shipments
                .GroupBy(s => s.Destination)
                .Select(g => new { Destination = g.Key, ShipmentCount = g.Count() })
                .OrderByDescending(x => x.ShipmentCount)
                .Take(topN)
                .ToListAsync();

            return query.Select(x => (x.Destination, x.ShipmentCount));
        }


    }
}
