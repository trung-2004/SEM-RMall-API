using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.FeedBacks;
using RMall.Models.General;
using RMall.Models.Shops;

namespace RMall.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public FeedbackController(RmallApiContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> GetAllFeedback()
        {
            try
            {
                List<Feedback> feedbacks = await _context.Feedbacks.OrderByDescending(s => s.Id).ToListAsync();
                List<FeedbackDTO> result = new List<FeedbackDTO>();
                foreach (Feedback feedback in feedbacks)
                {
                    result.Add(new FeedbackDTO
                    {
                        id = feedback.Id,
                        name = feedback.Name,
                        email = feedback.Email,
                        phone = feedback.Phone,
                        message = feedback.Message,
                        createdAt = feedback.CreatedAt,
                        updatedAt = feedback.UpdatedAt,
                        deletedAt = feedback.DeletedAt,
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var response = new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 400,
                    Message = ex.Message,
                    Data = ""
                };

                return BadRequest(response);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedBack(CreateFeedback model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Feedback feedback = new Feedback
                    {
                        Name = model.name,
                        Email = model.email,
                        Phone = model.phone,
                        Message = model.message,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    _context.Feedbacks.Add(feedback);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={feedback.Id}", new FeedbackDTO
                    {
                        id = feedback.Id,
                        name = feedback.Name,
                        email = feedback.Email,
                        phone = feedback.Phone,
                        message = feedback.Message,
                        createdAt = feedback.CreatedAt,
                        updatedAt = feedback.UpdatedAt,
                        deletedAt = feedback.DeletedAt,
                    });
                }
                catch (Exception ex)
                {
                    var response = new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = ex.Message,
                        Data = ""
                    };

                    return BadRequest(response);
                }
            }
            var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage);

            var validationResponse = new GeneralServiceResponse
            {
                Success = false,
                StatusCode = 400,
                Message = "Validation errors",
                Data = string.Join(" | ", validationErrors)
            };

            return BadRequest(validationResponse);
        }
    }
}
