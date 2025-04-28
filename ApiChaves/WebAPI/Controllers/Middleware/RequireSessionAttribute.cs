using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ApiChaves.WebAPI.Controllers.Middleware
{
    public class RequiresSessionAttribute : TypeFilterAttribute
    {
        public RequiresSessionAttribute() : base(typeof(RequiresSessionFilter))
        {
        }

        private class RequiresSessionFilter : IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var httpContext = context.HttpContext;
                if (httpContext.Session.GetString("Id") == null || httpContext.Session.GetString("Usuario") == null)
                {
                    context.Result = new RedirectToActionResult("Authenticate", "Login", null);
                }
            }
        }
    }

}
