using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]

    [ServiceFilter(typeof(CustomFilterAttribute<AppUser>))]
    [CustomAuthorize("RootAdmin")]
    public class UsersController : Controller
    {
        private readonly ResponseResult<AppUser> _responseResult;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor contextAccessor;
        public UsersController(UserManager<AppUser> _userManager, IHttpContextAccessor httpContextAccessor)
        {
            this._userManager = _userManager;
            this._responseResult = new ResponseResult<AppUser>();
            this.contextAccessor = httpContextAccessor;
        }


        [HttpGet("GetUserById/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {


            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                this._responseResult.Entity = user;
                return Ok(this._responseResult);
            }
            this._responseResult.HasError = true;
            this._responseResult.IsSuccess = false;
            this._responseResult.ErrorList.Add("User Not Found");
            return NotFound(this._responseResult);
        }
        [HttpGet("GetUserList")]
        public async Task<IActionResult> GetUserList()
        {
            this._responseResult.Entities = await _userManager.Users.ToListAsync();
            return Ok(this._responseResult);
        }
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var entity = await _userManager.FindByIdAsync(userId);
            if (entity == null)
            {
                this._responseResult.HasError = true;
                this._responseResult.IsSuccess = false;
                this._responseResult.ErrorList.Add("User Not Found");
                return NotFound(this._responseResult);
            }
            await _userManager.DeleteAsync(entity);
            return Ok(entity.Email + " Silindi");
        }
        //[AllowAnonymous]
        //[HttpGet("GenerateUser")]
        //public async Task<IActionResult> GenerateUser()
        //{
        //    var user = new AppUser()
        //    {
        //        NameSurname = "Serkan Afşar",
        //        Email = "serkan-afsar@hotmail.com",
        //        UserName = "serkan-afsar@hotmail.com"
        //    };

        //    await _userManager.CreateAsync(user, "1Q2w3E4r!");
        //    await _userManager.AddToRoleAsync(user, "RootAdmin");
        //    return Ok("Created");
        //}
    }
}
