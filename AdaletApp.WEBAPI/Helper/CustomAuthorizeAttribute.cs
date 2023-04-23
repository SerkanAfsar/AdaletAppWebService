using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace AdaletApp.WEBAPI.Utilities
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }
    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return;
            }

            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (!IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized.. Invalid EMail Or Password");

            }

            if (IsAuthenticated && _claim != null)
            {
                var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;
                bool flagClaim = false;
                foreach (var item in _claim)
                {
                    if (claimsIndentity.HasClaim(ClaimTypes.Role, item))
                    {
                        flagClaim = true;
                        break;

                    }

                }
                if (!flagClaim)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                }
            }
        }
    }
}
