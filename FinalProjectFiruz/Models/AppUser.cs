using Microsoft.AspNetCore.Identity;

namespace FinalProjectFiruz.Models
{
    public class AppUser : IdentityUser
    {
        public string Fullname { get; set; }


    }
}
