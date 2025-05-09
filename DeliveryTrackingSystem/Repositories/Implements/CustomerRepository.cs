using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Helper;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class CustomerRepository(AppDbContext context, AppDbContext dbContext) : GenericRepository<Customer>(context), ICustomerRepository
    {
        private readonly AppDbContext _context = dbContext;
        public async Task<IEnumerable<Shipment>> GetCustomerShipmentsAsync(int customerId)
        {
            return await _context.Shipments
                .Where(s => s.CustomerId == customerId)
                .Include(s => s.Customer)
                .Include(s => s.Driver)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShipmentStatusHistory>> GetShipmentStatusHistoryAsync(int shipmentId)
        {
            return await _context.ShipmentStatusHistories
                .Where(h => h.ShipmentId == shipmentId)
                .Include(h => h.Shipment)
                .ToListAsync();
        }

        public async Task<(int TotalShipments, decimal TotalCost, Dictionary<ShipmentStatus, int> StatusBreakdown)> GetCustomerShipmentSummaryAsync(int customerId)
        {
            var shipments = await _context.Shipments
                .Where(s => s.CustomerId == customerId)
                .ToListAsync();

            var totalShipments = shipments.Count;
            var totalCost = shipments.Sum(s => s.ShippingCost);
            var statusBreakdown = shipments
                .GroupBy(s => s.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            return (totalShipments, totalCost, statusBreakdown);
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            return await _context.Customers
                .Where(c => c.FullName.Contains(searchTerm) ||
                            c.Email.Contains(searchTerm) ||
                            c.PhoneNumber.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> FilterCustomersByShipmentCountAsync(int minShipments, int maxShipments)
        {
            return await _context.Customers
                .Include(c => c.Shipments)
                .Where(c => c.Shipments.Count >= minShipments && c.Shipments.Count <= maxShipments)
                .ToListAsync();
        }
    }
}
