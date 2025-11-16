using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Admin")]
    public class DashboardAdmApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
