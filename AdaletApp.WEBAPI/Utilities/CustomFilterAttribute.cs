using AdaletApp.DAL.Concrete.EFCore;
using AdaletApp.Entities;
using AdaletApp.WEBAPI.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace AdaletApp.WEBAPI.Utilities
{
    public class CustomFilterAttribute<T> : IActionFilter, IExceptionFilter where T : class
    {
        private readonly ResponseResult<T> responseResult;
        private readonly AppDbContext appDbContext;

        public CustomFilterAttribute(AppDbContext appContext)
        {
            this.responseResult = new ResponseResult<T>();
            this.appDbContext = appContext;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("id"))
            {
                int id = Convert.ToInt32(context.ActionArguments["id"]);

                var entity = this.appDbContext.Set<T>().Find(id);
                if (entity == null)
                {
                    this.responseResult.IsSuccess = false;
                    this.responseResult.HasError = true;
                    this.responseResult.ErrorList.Add("Object Not Found!");
                    this.responseResult.StatusCode = System.Net.HttpStatusCode.NotFound;
                    context.Result = new NotFoundObjectResult(this.responseResult);
                }
                else
                {
                    context.HttpContext.Items.Add("entity", entity);
                }
            }

            if (!context.ModelState.IsValid)
            {
                this.responseResult.IsSuccess = false;
                this.responseResult.HasError = true;
                context.ModelState.Select(a => a.Value).SelectMany(a => a.Errors).ToList().ForEach(item =>
                {
                    this.responseResult.ErrorList.Add(item.ErrorMessage);
                });
                this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(this.responseResult);
            }
        }



        public void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                this.responseResult.IsSuccess = false;
                this.responseResult.HasError = true;
                this.responseResult.ErrorList.Add(context.Exception.Message);
                this.responseResult.StatusCode = System.Net.HttpStatusCode.BadRequest;
                context.Result = new BadRequestObjectResult(this.responseResult);
            }
        }
    }
}
