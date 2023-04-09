using Microsoft.AspNetCore.Identity;

namespace P230_Pronia.Entities
{
    public class User:IdentityUser
    {
        public string Fullname { get; set; }
    }
}
