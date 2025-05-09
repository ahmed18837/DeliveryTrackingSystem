using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class ShipmentStatusHistoryRepository(AppDbContext context) : GenericRepository<ShipmentStatusHistory>(context), IShipmentStatusHistoryRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<IEnumerable<ShipmentStatusHistory>> GetStatusHistoryByShipmentIdAsync(int shipmentId)
        {
            return await _context.ShipmentStatusHistories
                .Where(h => h.ShipmentId == shipmentId)
                .Include(h => h.Shipment)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task<ShipmentStatusHistory> GetLatestStatusChangeAsync(int shipmentId)
        {
            return await _context.ShipmentStatusHistories
                .Where(h => h.ShipmentId == shipmentId)
                .Include(h => h.Shipment)
                .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShipmentStatusHistory>> FilterStatusHistoryAsync(StatusHistoryFilter filter)
        {
            var query = _context.ShipmentStatusHistories
                .Include(h => h.Shipment)
                .AsQueryable();

            if (filter.ShipmentId.HasValue)
                query = query.Where(h => h.ShipmentId == filter.ShipmentId.Value);
            if (filter.OldStatus.HasValue)
                query = query.Where(h => h.OldStatus == filter.OldStatus.Value);
            if (filter.NewStatus.HasValue)
                query = query.Where(h => h.NewStatus == filter.NewStatus.Value);
            if (filter.StartDate.HasValue)
                query = query.Where(h => h.ChangedAt >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(h => h.ChangedAt <= filter.EndDate.Value);

            return await query.OrderBy(h => h.ChangedAt).ToListAsync();
        }

        public async Task<(int TotalChanges, Dictionary<ShipmentStatus, int> StatusTransitionCounts)> GetStatusChangeStatisticsAsync(int? shipmentId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.ShipmentStatusHistories.AsQueryable();

            if (shipmentId.HasValue)
                query = query.Where(h => h.ShipmentId == shipmentId.Value);
            if (startDate.HasValue)
                query = query.Where(h => h.ChangedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(h => h.ChangedAt <= endDate.Value);

            var history = await query.ToListAsync();

            var totalChanges = history.Count;
            var statusTransitionCounts = history
                .GroupBy(h => h.NewStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            return (totalChanges, statusTransitionCounts);
        }
    }
}
