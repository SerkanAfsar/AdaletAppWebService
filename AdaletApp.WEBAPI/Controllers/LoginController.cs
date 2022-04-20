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
    [ServiceFilter(typeof(CustomFilterAttribute<UserLoginViewModel>))]
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly UserManager<AppUser> userManager;

        public LoginController(ILoginService _loginService, UserManager<AppUser> _usermanager)
        {
            loginService = _loginService;
            userManager = _usermanager;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            var result = await loginService.TokenResult(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(a => a.Errors).Select(a => a.ErrorMessage).ToList());
            }

            var appUser = new AppUser
            {
                Email = model.EMail,
                UserName = model.EMail
            };
            var result = await userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                return Ok(appUser);

            }
            else
            {

                return BadRequest(result.Errors.Select(a => a.Description).ToList());
            }

        }
    }
}

