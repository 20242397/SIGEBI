using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Filters;

[AuthFilter]
[RoleFilter("Docente")]
public class DashboardDocController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
