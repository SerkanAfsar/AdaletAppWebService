using Microsoft.AspNetCore.Identity;


namespace AdaletApp.Entities
{
    public class AppUser : IdentityUser
    {
        public string NameSurname { get; set; }
    }
}
