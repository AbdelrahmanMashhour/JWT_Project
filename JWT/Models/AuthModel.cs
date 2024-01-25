using System.ComponentModel.DataAnnotations;

namespace testApi.Models
{
    public class AuthModel
    {
        [Required]
        public string Message { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime ExpireOn { get; set; }
        public List<string> Roles { get; set; }
    }
}
