using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;

namespace RMall.Controllers
{
    [Route("api/language")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public LanguageController(RmallApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguageAll()
        {
            try
            {
                List<Language> languages = await _context.Languages.OrderByDescending(m => m.Name).ToListAsync();
                List<LanguageDTO> result = new List<LanguageDTO>();
                foreach (var language in languages)
                {
                    result.Add(new LanguageDTO
                    {
                        id = language.Id,
                        name = language.Name,
                        createdAt = language.CreatedAt,
                        updatedAt = language.UpdatedAt,
                        deletedAt = language.DeletedAt,
                    });
                }
                return Ok(result);
            } catch (Exception ex)
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
    }
}
