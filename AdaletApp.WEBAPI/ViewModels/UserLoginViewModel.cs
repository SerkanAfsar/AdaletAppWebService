using AdaletApp.Entities;
using System.ComponentModel.DataAnnotations;

namespace AdaletApp.WEBAPI.ViewModels
{
    public class UserLoginViewModel
    {
     
        [Required(ErrorMessage = "EMail is Required")]
        public string EMail { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "RePassword is Required")]
        [Compare("Password", ErrorMessage = "Password and RePassword must be matched")]
        public string RePassword { get; set; }
        
    }
}
