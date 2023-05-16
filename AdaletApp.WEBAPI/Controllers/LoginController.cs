using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using AdaletApp.WEBAPI.Utilities;
using AdaletApp.WEBAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [CustomAuthorize("RootAdmin")]
    [ServiceFilter(typeof(CustomFilterAttribute<UserRegisterDTO>))]
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ResponseResult<UserRegisterDTO> responseResult;

        public LoginController(ILoginService _loginService, UserManager<AppUser> _usermanager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            this.loginService = _loginService;
            this.userManager = _usermanager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.responseResult = new ResponseResult<UserRegisterDTO>();

        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var result = await loginService.TokenResult(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(CustomFilterAttribute<UserRegisterDTO>))]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserRegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                this.responseResult.ErrorList = ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage).ToList();
                return BadRequest(this.responseResult);
            }

            var appUser = new AppUser
            {
                Email = model.EMail,
                UserName = model.EMail,
                NameSurname = model.NameSurname
            };
            var result = await userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await userManager.AddToRoleAsync(appUser, model.RoleName);
                if (roleResult.Succeeded)
                {
                    return Ok(model.EMail + " Oluşturuldu");
                }
                else
                {
                    await userManager.DeleteAsync(appUser);
                    this.responseResult.ErrorList = (roleResult.Errors.Select(a => a.Description).ToList());
                    return BadRequest(this.responseResult);
                }

            }
            else
            {
                this.responseResult.ErrorList = (result.Errors.Select(a => a.Description).ToList());
                return BadRequest(this.responseResult);
            }

        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return Ok(this.responseResult);
        }
    }
}

