using System.ComponentModel.DataAnnotations;

namespace AdaletApp.WEBAPI.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "EMail is Required")]
        public string EMail { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role Type is Required")]
    }
}
