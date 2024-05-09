using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class LoggingController : Controller
{
    private readonly IUserLoggingService _userLoggingService;

    public LoggingController(IUserLoggingService userLoggingService)
    {
        _userLoggingService = userLoggingService;
    }

    [HttpGet]
    public ViewResult ViewLogs()
    {
        var items = _userLoggingService.GetAllLogs().Select(l => new UserLogListItemViewModel()
        {
            Id = l.Id,
            UserId = l.UserId,
            Message = l.Message,
            Created = l.Created,
            Action = l.Action,
        });

        UserLogListViewModel model = new UserLogListViewModel()
        {
            Items = items.ToList(),
        };

        return View(model);
    }
}
