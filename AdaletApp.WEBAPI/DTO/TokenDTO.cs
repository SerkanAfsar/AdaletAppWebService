namespace AdaletApp.WEBAPI.ViewModels
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string NameSurname { get; set; }
        public string EMail { get; set; }
    }
}
