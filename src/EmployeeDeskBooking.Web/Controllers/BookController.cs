using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeDeskBooking.Application.Auth;

namespace EmployeeDeskBooking.Web.Controllers;

[Authorize(Roles = AuthRoles.Employee)]
public class BookController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
