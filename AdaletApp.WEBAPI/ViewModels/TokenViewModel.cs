namespace AdaletApp.WEBAPI.ViewModels
{
    public class TokenViewModel
    {
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string NameSurname { get; set; }
        public string EMail { get; set; }
    }
}
