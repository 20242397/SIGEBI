using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Docente")]
    public class DashboardDocWebController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}