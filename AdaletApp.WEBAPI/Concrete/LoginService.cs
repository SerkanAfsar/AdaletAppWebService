using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

            if (await userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                this.responseResult.ErrorList.Add("Şifre Yanlış!");
                return this.responseResult;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JWT:Secret"]);
            var claims = new Dictionary<string, object>();
            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add("Role", role);

            }
            claims.Add("ID", user.Id);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenKey = tokenHandler.WriteToken(token);
            this.responseResult.IsSuccess = true;
            this.responseResult.HasError = false;
            this.responseResult.Entity = new TokenViewModel { Token = tokenKey, ExpireDate = DateTime.Now.AddDays(7) };
            return responseResult;

        }
    }
}
