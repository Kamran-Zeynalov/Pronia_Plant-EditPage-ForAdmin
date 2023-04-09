using System.ComponentModel.DataAnnotations;

namespace P230_Pronia.ViewModels
{
    public class LoginVM
    {
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
