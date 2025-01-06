using Microsoft.AspNetCore.Identity;

namespace MagicVillaWeb.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name {  get; set; }
    }
}
