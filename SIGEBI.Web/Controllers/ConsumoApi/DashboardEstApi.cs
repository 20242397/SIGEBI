using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

namespace SIGEBI.Web.Controllers.ConsumoApi
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
