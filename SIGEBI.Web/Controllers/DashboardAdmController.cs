using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
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
