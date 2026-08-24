using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeDeskBooking.Application.Auth;

namespace EmployeeDeskBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AuthRoles.Admin)]
public class BookingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
