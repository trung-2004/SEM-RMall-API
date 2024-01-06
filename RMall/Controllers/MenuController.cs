﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Users;
using System.Security.Claims;

namespace RMall.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public MenuController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMenu()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
            }

            try
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Incorrect current password",
                        Data = ""
                    });
                }

                if (user.Role.Contains("Super Admin"))
                {
                    var menu = new List<MenuItem>
                    {
                        new MenuItem { Title = "Dashboard", Url = "/", Icon = "<i class=\"feather-grid\"></i>" },
                        new MenuItem { Title = "Exam", Url = "/exam-list", Icon = "<i class=\"fas fa-clipboard-list\"></i>" },
                        new MenuItem { Title = "Test", Url = "/test-list", Icon = "<i class=\"feather-calendar\"></i>"},
                        new MenuItem { Title = "Class", Url = "/classes-list", Icon = "<i class=\"fas fa-building\"></i>"},
                        new MenuItem { Title = "Course", Url = "/course-list", Icon = "<i class=\"fas fa-book-reader\"></i>"},
                        new MenuItem { Title = "Class Course", Url = "/courseclass-list", Icon = "<i class=\"feather-server\"></i>"},
                        new MenuItem { Title = "Question", Url = "/question-list", Icon = "<i class=\"feather-inbox\"></i>"},
                        new MenuItem { Title = "Student", Url = "/student-list", Icon = "<i class=\"fas fa-graduation-cap\"></i>"},
                        new MenuItem { Title = "Staff", Url = "/teacher-list", Icon = "<i class=\"fas fa-chalkboard-teacher\"></i>"},
                        new MenuItem { Title = "Profile", Url = "/profile", Icon = "<i class=\"fas fa-cog\"></i>"},
                        // Thêm các mục menu khác cho Admin
                    };
                    return Ok(menu);
                }
                else if (user.Role.Contains("Shopping Center Manager Staff"))
                {
                    var menu = new List<MenuItem>
                    {
                        new MenuItem { Title = "Dashboard", Url = "/staff-dashboard", Icon = "<i class=\"feather-grid\"></i>" },
                        new MenuItem { Title = "Exam", Url = "/exam-list", Icon = "<i class=\"fas fa-clipboard-list\"></i>" },
                        new MenuItem { Title = "Test", Url = "/test-list", Icon = "<i class=\"feather-calendar\"></i>"},
                        new MenuItem { Title = "Class", Url = "/classes-list", Icon = "<i class=\"fas fa-building\"></i>"},
                        new MenuItem { Title = "Course", Url = "/course-list", Icon = "<i class=\"fas fa-book-reader\"></i>"},
                        new MenuItem { Title = "Class Course", Url = "/courseclass-list", Icon = "<i class=\"feather-server\"></i>"},
                        new MenuItem { Title = "Question", Url = "/question-list", Icon = "<i class=\"feather-inbox\"></i>"},
                        new MenuItem { Title = "Student", Url = "/student-list", Icon = "<i class=\"fas fa-graduation-cap\"></i>"},
                        new MenuItem { Title = "Profile", Url = "/profile", Icon = "<i class=\"fas fa-cog\"></i>"},



                        // Thêm các mục menu khác cho User
                        //
                    };
                    return Ok(menu);
                }
                else if (user.Role.Contains("Movie Theater Manager Staff"))
                {
                    var menu = new List<MenuItem>
                    {
                        new MenuItem { Title = "Dashboard", Url = "/teacher-dashboard", Icon = "<i class=\"feather-grid\"></i>" },
                        new MenuItem { Title = "Test", Url = "/testbyteacher-list", Icon = "<i class=\"feather-calendar\"></i>"},
                        new MenuItem { Title = "Class", Url = "/classesbyteacher-list", Icon = "<i class=\"fas fa-building\"></i>"},
                        new MenuItem { Title = "Profile", Url = "/profile", Icon = "<i class=\"fas fa-cog\"></i>"},


                        // Thêm các mục menu khác cho User
                    };
                    return Ok(menu);
                }
                else
                {
                    // Mặc định cho các trường hợp khác
                    var menu = new List<MenuItem>();
                    return Ok(menu);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = ex.Message,
                });
            }
        }
    }
}
