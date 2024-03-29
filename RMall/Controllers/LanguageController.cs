﻿using Microsoft.AspNetCore.Http;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetLanguageAll()
        {
            try
            {
                List<Language> languages = await _context.Languages.OrderBy(m => m.Name).ToListAsync();
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

        [HttpGet("get-by-movie/{id}")]
        public async Task<IActionResult> GetLanguageAll(int id)
        {
            try
            {
                List<MovieLanguage> languages = await _context.MovieLanguages.Where(l => l.MovieId == id).ToListAsync();
                List<LanguageDTO> result = new List<LanguageDTO>();
                foreach (var item in languages)
                {
                    var language = await _context.Languages.FindAsync(item.LanguageId);
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
    }
}
