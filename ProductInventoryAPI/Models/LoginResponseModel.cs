namespace ProductInventoryAPI.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

}
