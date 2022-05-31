using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdaletApp.WEBAPI.Concrete
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly ResponseResult<TokenViewModel> responseResult;
        public LoginService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.responseResult = new ResponseResult<TokenViewModel>()
            {
                Entities = null,
                HasError = true,
                IsSuccess = false,

            };
        }
        public async Task<ResponseResult<TokenViewModel>> TokenResult(UserLoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
            {
                this.responseResult.ErrorList.Add("Bilgileri Giriniz!");
                return this.responseResult;
            }

            var user = await userManager.FindByEmailAsync(loginViewModel.EMail);
            if (user == null)
            {
                this.responseResult.ErrorList.Add("Kullanıcı Bulunamadı!");
                return this.responseResult;
            }

            if (await userManager.CheckPasswordAsync(user, loginViewModel.Password) == false)
            {
                this.responseResult.ErrorList.Add("Şifre Yanlış!");
                return this.responseResult;
            }

            var claims = new List<Claim>();
            claims.Add(new Claim("ID", user.Id));
            claims.Add(new Claim("UserName", user.NameSurname));

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken
            (
                 configuration["JWT:ValidIssuer"],
                 configuration["JWT:ValidAudience"],
                 claims,
                 expires: DateTime.UtcNow.AddDays(7),
                 signingCredentials: signIn

            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = tokenHandler.WriteToken(token);
            this.responseResult.IsSuccess = true;
            this.responseResult.HasError = false;
            this.responseResult.Entity = new TokenViewModel { Token = tokenKey, ExpireDate = DateTime.Now.AddDays(7), NameSurname = user.NameSurname, EMail = user.Email };
            return responseResult;

        }
    }
}
