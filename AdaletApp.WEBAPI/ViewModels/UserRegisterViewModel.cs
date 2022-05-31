using System.ComponentModel.DataAnnotations;

namespace AdaletApp.WEBAPI.ViewModels
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Name Surname is Required")]
        public string NameSurname { get; set; }
        [Required(ErrorMessage = "EMail is Required")]
        public string EMail { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "RePassword is Required")]
        [Compare("Password", ErrorMessage = "Password and Repassword Must Range")]
        public string RePassword { get; set; }
        [Required(ErrorMessage = "Select Role")]
        public string RoleName { get; set; }

    }
}
