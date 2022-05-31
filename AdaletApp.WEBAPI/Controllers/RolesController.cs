using AdaletApp.Entities;
using AdaletApp.WEBAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AdaletApp.WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [CustomAuthorize("RootAdmin")]
    [ServiceFilter(typeof(CustomFilterAttribute<AppRole>))]
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> roleManager;
        private readonly ResponseResult<AppRole> responseResult;
        public RolesController(RoleManager<AppRole> roleManager)
        {
            this.roleManager = roleManager;
            this.responseResult = new ResponseResult<AppRole>();

        }
        [HttpGet("CreateAllRoles")]
        public async Task<IActionResult> CreateAllRoles()
        {
            foreach (string name in Enum.GetNames(typeof(RoleTypes)))
            {
                var role = new AppRole() { Name = name, CreateDate = DateTime.Now };
                try
                {
                    await this.roleManager.CreateAsync(role);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }

            }
            return Ok("Roles Created");
        }

        [HttpGet("GetRoleById/{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var result = await roleManager.FindByIdAsync(roleId);
            if (result == null)
            {
                this.responseResult.ErrorList.Add("Role Not Found");
                return NotFound(this.responseResult);
            }
            this.responseResult.Entity = result;
            return Ok(this.responseResult);
        }
        [AllowAnonymous]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            this.responseResult.Entities = await roleManager.Roles.ToListAsync();
            return Ok(this.responseResult);
        }
    }
}
