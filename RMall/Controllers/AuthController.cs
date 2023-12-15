using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMall.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IConfiguration _config;
        public AuthController(RmallApiContext context, IConfiguration config) 
        { 
            _context = context;
            _config = config;
        }

        private string GenerateToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var signatureKey = new SigningCredentials(secretKey,
                                    SecurityAlgorithms.HmacSha256);
            var payload = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.Fullname),
                new Claim(ClaimTypes.Role,user.Role),
            };
            var token = new JwtSecurityToken(
                    _config["JWT:Issuer"],
                    _config["JWT:Audience"],
                    payload,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: signatureKey
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(model.email));
                if (user == null)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Invalid email/password"
                    });
                }
                bool verified = BCrypt.Net.BCrypt.Verify(model.password, user.Password);
                if (!verified)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Invalid email/password"
                    });
                }

                return Ok(new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Authenticate success",
                    Data = GenerateToken(user)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {

                // Kiểm tra xem email đã tồn tại trong cơ sở dữ liệu hay chưa
                bool emailExists = await _context.Users.AnyAsync(c => c.Email == model.email);

                if (emailExists)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Student email already exists",
                        Data = ""
                    });
                }

                // hash password
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var hassPassword = BCrypt.Net.BCrypt.HashPassword(model.password, salt);


                User data = new User
                {
                    Fullname = model.fullname,
                    Email = model.email,
                    Password = hassPassword,
                    Status = 0,
                    Role = "User",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                };

                _context.Users.Add(data);
                await _context.SaveChangesAsync();

                // start send mail

                /*Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = data.Email;
                mailrequest.Subject = "Welcome to Examonimy";
                mailrequest.Body = EmailContentRegister.GetHtmlcontentRegisterStudent(data.Fullname, data.Email, password);

                await _emailService.SendEmailAsync(mailrequest);*/

                // end send mail


                return Created($"get-by-id?id={data.Id}", new UserDTO
                {
                    id = data.Id,
                    fullname = data.Fullname,
                    birthday = data.Birthday,
                    email = data.Email,
                    phone = data.Phone,
                    status = data.Status,
                    role = data.Role,
                    createdAt = data.CreatedAt,
                    updatedAt = data.UpdatedAt,
                    deletedAt = data.DeletedAt,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "Not Authorized",
                    Data = ""
                });
            }

            try
            {

                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
                if (user != null)
                {
                    bool verified = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password);

                    // Kiểm tra mật khẩu hiện tại
                    if (verified)
                    {
                        // hash password
                        var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                        var hassNewPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, salt);
                        // Thực hiện thay đổi mật khẩu
                        user.Password = hassNewPassword;
                        _context.SaveChanges();
                        return Ok(new GeneralServiceResponse
                        {
                            Success = true,
                            StatusCode = 200,
                            Message = "Password changed successfully",
                            Data = ""
                        });
                    }
                    else
                    {
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Incorrect current password",
                            Data = ""
                        });
                    }
                }
                else
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Incorrect current password",
                        Data = ""
                    });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => s.Email.Equals(model.email));
                if (user == null)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Student not found"
                    });
                }

                var resetToken = GenerateResetToken();
                user.ResetToken = resetToken;
                user.ResetTokenExpiry = DateTime.Now.AddHours(1); // Thời gian hết hiệu lực của token: 1 giờ
                await _context.SaveChangesAsync();

                var resetLink = "http://localhost:3000/reset-password/" + resetToken;

                /*Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = student.Email;
                mailrequest.Subject = "Password Reset";
                mailrequest.Body = $"Click the link to reset your password: {resetLink}";

                await _emailService.SendEmailAsync(mailrequest);*/


                return Ok(new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Password reset email sent successfully"
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("reset-password/{token}")]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => s.Email.Equals(model.Email));
                if (user == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Student not found"
                    });
                }

                // Kiểm tra tính hợp lệ của mã reset
                if (model == null || string.IsNullOrEmpty(token) || user.ResetToken != token || user.Email != model.Email || user.ResetTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid or expired reset token"
                    });
                }

                // Cập nhật mật khẩu
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var hassNewPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, salt);

                user.Password = hassNewPassword; // Hash mật khẩu trước khi lưu
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                await _context.SaveChangesAsync();

                return Ok(new GeneralServiceResponse
                {
                    Success = true,
                    Message = "Password reset successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {// get info form token
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
            }

            try
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

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

                return Ok(new ProfileRespone // đúng ra phải là UserProfileDTO
                {
                    id = user.Id,
                    email = user.Email,
                    fullname = user.Fullname,
                    birthday = user.Birthday,
                    phone = user.Phone,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 404,
                    Message = ex.Message,
                    Data = ""
                });
            }
        }

        [HttpPut]
        [Route("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest model)
        {
            if (ModelState.IsValid)
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (!identity.IsAuthenticated)
                {
                    return Unauthorized(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 401,
                        Message = "Not Authorized",
                        Data = ""
                    });
                }

                try
                {
                    var userClaims = identity.Claims;
                    var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                    if (user == null)
                    {
                        return Unauthorized(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 401,
                            Message = "Not Authorized",
                            Data = ""
                        });
                    }

                    user.Birthday = model.birthday;
                    user.Phone = model.phone;
                    user.UpdatedAt = DateTime.Now;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    return NoContent();

                }
                catch (Exception ex)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = ex.Message,
                        Data = ""
                    });
                }
            }

            return BadRequest(new GeneralServiceResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "",
                Data = ""
            });

        }
    }
}
