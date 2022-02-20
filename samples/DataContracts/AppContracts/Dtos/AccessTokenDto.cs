namespace AppContracts.Dtos
{
    public class AccessTokenDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Role { get; set; }
        public string CreateAt { get; set; }
        public string ExpireAt { get; set; }
    }
}
