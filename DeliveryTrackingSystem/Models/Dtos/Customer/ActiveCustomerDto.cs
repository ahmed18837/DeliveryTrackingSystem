namespace DeliveryTrackingSystem.Models.Dtos.Customer
{
    public class ActiveCustomerDto
    {
        public CustomerDto Customer { get; set; }
        public int ShipmentCount { get; set; }
    }
}