using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.Foods;
using RMall.Models.General;
using RMall.Service.UploadFiles;

namespace RMall.Controllers
{
    [Route("api/food")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IImgService _imgService;
        public FoodController(RmallApiContext context, ImgService imgService)
        {
            _context = context;
            _imgService = imgService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> getAllFood()
        {
            try
            {
                List<Food> foods = await _context.Foods.Where(f => f.DeletedAt == null).OrderByDescending(f => f.Id).ToListAsync();
                List<FoodDTO> result = new List<FoodDTO>();
                foreach (var item in foods)
                {
                    result.Add(new FoodDTO
                    {
                        id = item.Id,
                        name = item.Name,
                        image = item.Image,
                        price = item.Price,
                        quantity = item.Quantity,
                        createdAt = item.CreatedAt,
                        updatedAt = item.UpdatedAt,
                        deletedAt = item.DeletedAt,
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

        [HttpGet("trash-can")]
        public async Task<IActionResult> TrashCan()
        {
            try
            {
                List<Food> foods = await _context.Foods.Where(f => f.DeletedAt != null).OrderByDescending(f => f.Id).ToListAsync();
                List<FoodDTO> result = new List<FoodDTO>();
                foreach (var item in foods)
                {
                    result.Add(new FoodDTO
                    {
                        id = item.Id,
                        name = item.Name,
                        image = item.Image,
                        price = item.Price,
                        quantity = item.Quantity,
                        createdAt = item.CreatedAt,
                        updatedAt = item.UpdatedAt,
                        deletedAt = item.DeletedAt,
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
        public async Task<IActionResult> CreateFood(CreateFood model)
        {
            try
            {
                var imageUrl = await _imgService.UploadImageAsync(model.image);
                if (imageUrl != null)
                {
                    Food food = new Food
                    {
                        Name = model.name,
                        Image = imageUrl,
                        Price = model.price,
                        Quantity = model.quantity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };

                    _context.Foods.Add(food);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={food.Id}", new FoodDTO
                    {
                        id = food.Id,
                        name = food.Name,
                        price = food.Price,
                        quantity = food.Quantity,
                        createdAt = food.CreatedAt,
                        updatedAt = food.UpdatedAt,
                        deletedAt = food.DeletedAt,
                    });
                }
                else
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Please provide an avatar.",
                        Data = ""
                    });
                }

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

        [HttpPut("edit")]
        public async Task<IActionResult> EditFood(EditFood model)
        {
            try
            {
                Food foodExisting = await _context.Foods.FindAsync(model.id);
                if (foodExisting == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }

                Food food = new Food
                {
                    Id = model.id,
                    Name = model.name,
                    Price = model.price,
                    Quantity = model.quantity,
                    CreatedAt = foodExisting.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null
                };

                if (model.image != null)
                {
                    string imageUrl = await _imgService.UploadImageAsync(model.image);

                    if (imageUrl == null)
                    {
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Failed to upload avatar.",
                            Data = ""
                        });
                    }

                    food.Image = imageUrl;
                }
                else
                {
                    food.Image = foodExisting.Image;
                }

                _context.Foods.Update(food);
                await _context.SaveChangesAsync();

                return Ok(new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Edit successfully",
                    Data = ""
                });

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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                Food food = await _context.Foods.FindAsync(id);

                if (food != null)
                {
                    food.DeletedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Soft delete successful",
                    Data = ""
                };

                return Ok(response);


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

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Food food = await _context.Foods.FindAsync(id);
                if (food == null)
                {
                    return NotFound();
                }

                food.DeletedAt = null;

                _context.Foods.Update(food);
                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "restore successful",
                    Data = ""
                };

                return Ok(response);

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
