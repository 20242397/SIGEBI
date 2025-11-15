using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers.Integracion
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class DashboardAdmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
