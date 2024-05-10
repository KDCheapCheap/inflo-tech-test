﻿using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models;

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
        var items = _userLoggingService.GetAllLogs().Select(l => new UserLogListItemViewModel(l));

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
        var items = _userLoggingService.GetAllLogsForUser(userId).Select(l => new UserLogListItemViewModel(l));

        UserLogListViewModel model = new UserLogListViewModel()
        {
            Items = items.ToList(),
            UsersName = items.ToList()[0].UserName
        };

        return View(model);
    }

    [HttpGet]
    [Route("LogSummary/{logId}")]
    [ActionName("LogSummary")]
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
