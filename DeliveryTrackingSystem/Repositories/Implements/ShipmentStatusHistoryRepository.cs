using DeliveryTrackingSystem.Data;
using DeliveryTrackingSystem.Models.Entities;
using DeliveryTrackingSystem.Repositories.GenericRepository;
using DeliveryTrackingSystem.Repositories.Interfaces;

namespace DeliveryTrackingSystem.Repositories.Implements
{
    public class ShipmentStatusHistoryRepository : GenericRepository<ShipmentStatusHistory>, IShipmentStatusHistoryRepository
    {
        public ShipmentStatusHistoryRepository(AppDbContext context) : base(context) { }
    }
}
