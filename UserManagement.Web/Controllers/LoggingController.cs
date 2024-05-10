using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Controllers;

[Route("logs")]
public class LoggingController : Controller
{
    private readonly IUserLoggingService _userLoggingService;
    private readonly IUserService _userService;

    public LoggingController(IUserLoggingService userLoggingService, IUserService userService)
    {
        _userLoggingService = userLoggingService;
        _userService = userService;
    }

    [HttpGet]
    [Route("ViewLogs")]
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

    [HttpGet]
    [Route("ViewLogs/{userId}")]
    [ActionName("ViewLogs")]
    public ViewResult ViewLogsForUser(long userId)
    {
        User user = _userService.GetUserById(userId);

        var items = _userLoggingService.GetAllLogsForUser(userId).Select(l => new UserLogListItemViewModel()
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
            UsersName = $"{user.Forename} {user.Surname}"
        };

        return View(model);
    }

    public ViewResult ViewLogSummary(long logId)
    {
        var log = _userLoggingService.GetLogById(logId);

        var logModel = new LogSummaryViewModel
        {
            Log = log
        };

        return View(logModel);
    }
}
