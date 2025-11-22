using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers
{
    [AuthFilter]
    [RoleFilter("Estudiante")]
    public class DashboardEstApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
