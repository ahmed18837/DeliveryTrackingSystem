namespace DeliveryTrackingSystem.Models.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? ProfileImageFileName { get; set; }
        //public string ApplicationUserId { get; set; }
    }
}