namespace Inventory.WebApi.Models
{
    public class TokenRequestModel
    {
        public string ApiKey { get; set; }
        public string Signature { get; set; }
    }
}