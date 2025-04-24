namespace DeliveryTrackingSystem.Models.Dtos.Auth
{
    public class ResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
