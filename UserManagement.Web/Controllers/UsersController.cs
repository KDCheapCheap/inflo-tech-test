using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserManagement.Data.Entities;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly IUserLoggingService _userLoggingService;

    public UsersController(IUserService userService, ILogger<UsersController> logger, IUserLoggingService userLoggingService)
    {
        _userService = userService;
        _logger = logger;
        _userLoggingService = userLoggingService;
    }

    [HttpGet]
    [Route("List")]
    public ViewResult List()
    {
        var items = _userService.GetAll().Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("List/{active}")]
    public ViewResult List(bool active)
    {
        var items = _userService.FilterByActive(active).Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            DateOfBirth = p.DateOfBirth,
            IsActive = p.IsActive
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    [Route("NewUser")]
    [ActionName("CreateUser")]
    public ViewResult CreateUserPage()
    {
        return View();
    }

    [HttpPost]
    [Route("CreateUser")]
    [ActionName("CreateUser")]
    public IActionResult CreateUser(User newUser)
    {
        try
        {
            var createdUser = _userService.CreateUser(newUser);

            UserLog createLog = new UserLog()
            {
                UserId = createdUser.Id,
                Action = Data.Enums.UserLogAction.Add,
                Created = DateTime.UtcNow,
                Message = $"Created {createdUser.Forename} {createdUser.Surname}",
                AfterChange = JsonConvert.SerializeObject(createdUser)
            };

            _userLoggingService.CreateLogEntry(createLog);

            return RedirectToAction("View", new { id = createdUser.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Creating User. Message: {ex.Message} Stack trace: {ex.StackTrace}");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("View/{userId}")]
    [ActionName("UserSummary")]
    public ViewResult ViewUser(long userId)
    {
        var user = _userService.GetUserById(userId);

        var userModel = new UserSummaryViewModel
        {
            user = new UserListItemViewModel
            {
                Id = user.Id,
                Forename = user.Forename,
                Surname = user.Surname,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive
            }
        };

        return View(userModel);
    }

    [HttpGet]
    [Route("EditUser/{userId}")]
    [ActionName("EditUser")]
    public ViewResult EditUserPage(long userId)
    {
        var userToEdit = _userService.GetUserById(userId);

        if (userToEdit == null)
        {
            RedirectToAction("List");
            return View();
        }

        return View(userToEdit);
    }

    [HttpPost]
    [Route("UpdateUser")]
    [ActionName("UpdateUser")]
    public IActionResult EditUser(User userToEdit)
    {
        string userBeforeChange = JsonConvert.SerializeObject(_userService.GetUserByIdUntracked(userToEdit.Id));
        
        try
        {
            UserLog editLog = new UserLog()
            {
                UserId = userToEdit.Id,
                Action = Data.Enums.UserLogAction.Edit,
                Created = DateTime.UtcNow,
                Message = $"Edited User ID: {userToEdit.Id}",
                BeforeChange = userBeforeChange,
            };

            var editedUser = _userService.UpdateUser(userToEdit);

            editLog.AfterChange = JsonConvert.SerializeObject(editedUser);

            _userLoggingService.CreateLogEntry(editLog);

            return RedirectToAction("View", new { id = editedUser.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error Editing User. Message: {ex.Message} Stack trace: {ex.StackTrace}");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("Delete/{userId}")]
    [ActionName("DeleteUser")]
    public IActionResult DeleteUser(long userId)
    {
        try
        {
            User userBeforeDeletion = _userService.GetUserById(userId);
            _userService.DeleteUser(userId);

            UserLog deleteLog = new UserLog()
            {
                UserId = userBeforeDeletion.Id,
                Action = Data.Enums.UserLogAction.Edit,
                Created = DateTime.UtcNow,
                Message = $"Deleted {userBeforeDeletion.Forename} {userBeforeDeletion.Surname}",
                BeforeChange = JsonConvert.SerializeObject(userBeforeDeletion),
                AfterChange = string.Empty
            };

            _userLoggingService.CreateLogEntry(deleteLog);

            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting user. Message: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}
