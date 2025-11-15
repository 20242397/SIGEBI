using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

[AuthFilter]
[RoleFilter("Estudiante")]
public class DashboardEstController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
