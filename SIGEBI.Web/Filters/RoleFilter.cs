using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SIGEBI.Web.Filters
{
    public class RoleFilter : ActionFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        public RoleFilter(params string[] roles)
        {
            _rolesPermitidos = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var rol = context.HttpContext.Session.GetString("UserRole");

            if (rol == null || !_rolesPermitidos.Contains(rol))
            {
                context.Result = new RedirectToActionResult("NoAutorizado", "Auth", null);
            }
        }
    }
}
