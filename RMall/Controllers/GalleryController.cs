using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.Foods;
using RMall.Models.GalleryMalls;
using RMall.Models.General;
using RMall.Service.UploadFiles;

namespace RMall.Controllers
{
    [Route("api/gallery")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IImgService _imgService;
        public GalleryController(RmallApiContext context, IImgService imgService)
        {
            _context = context;
            _imgService = imgService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> getAllGalleries()
        {
            try
            {
                List<GalleryMall> galleries = await _context.GalleryMalls.Where(f => f.DeletedAt == null).OrderByDescending(f => f.Id).ToListAsync();
                List<GalleryDTO> result = new List<GalleryDTO>();
                foreach (var item in galleries)
                {
                    result.Add(new GalleryDTO
                    {
                        id = item.Id,
                        imagePath = item.ImagePath,
                        productName = item.ProductName,
                        description = item.Description,
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

        [HttpGet("trash-can")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> TrashCan()
        {
            try
            {
                List<GalleryMall> galleries = await _context.GalleryMalls.Where(f => f.DeletedAt != null).OrderByDescending(f => f.DeletedAt).ToListAsync();
                List<GalleryDTO> result = new List<GalleryDTO>();
                foreach (var item in galleries)
                {
                    result.Add(new GalleryDTO
                    {
                        id = item.Id,
                        imagePath = item.ImagePath,
                        productName = item.ProductName,
                        description = item.Description,
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

        [HttpGet("get-by-id/{id}")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> getGalleryById(int id)
        {
            try
            {
                GalleryMall galleryMall = await _context.GalleryMalls.FirstOrDefaultAsync(f => f.Id == id && f.DeletedAt == null);
                if (galleryMall != null)
                {
                    return Ok(new GalleryDTO
                    {
                        id = galleryMall.Id,
                        productName = galleryMall.ProductName,
                        imagePath = galleryMall.ImagePath,
                        description = galleryMall.Description,
                        createdAt = galleryMall.CreatedAt,
                        updatedAt = galleryMall.UpdatedAt,
                        deletedAt = galleryMall.DeletedAt

                    });
                }
                else
                {
                    var response = new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    };

                    return NotFound(response);
                }
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
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> CreateGallery([FromForm] CreateGalleryMall model)
        {
            try
            {
                var imageUrl = await _imgService.UploadImageAsync(model.imagePath, "galleryMalls");
                if (imageUrl != null)
                {
                    GalleryMall galleryMall = new GalleryMall
                    {
                        ProductName = model.productName,
                        ImagePath = imageUrl,
                        Description = model.description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };

                    _context.GalleryMalls.Add(galleryMall);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={galleryMall.Id}", new GalleryDTO
                    {
                        id = galleryMall.Id,
                        productName = galleryMall.ProductName,
                        imagePath = imageUrl,
                        description = galleryMall.Description,
                        createdAt = galleryMall.CreatedAt,
                        updatedAt = galleryMall.UpdatedAt,
                        deletedAt = galleryMall.DeletedAt,
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

        [HttpPut("edit")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> EditGallery([FromForm] EditGalleryMall model)
        {
            try
            {
                GalleryMall galleryMallExisting = await _context.GalleryMalls.FindAsync(model.id);
                if (galleryMallExisting == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }

                galleryMallExisting.ProductName = model.productName;
                galleryMallExisting.Description = model.description;
                galleryMallExisting.UpdatedAt = DateTime.Now;

                if (model.imagePath != null)
                {
                    string imageUrl = await _imgService.UploadImageAsync(model.imagePath, "galleryMalls");

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

                    galleryMallExisting.ImagePath = imageUrl;
                }
                else
                {
                    galleryMallExisting.ImagePath = galleryMallExisting.ImagePath;
                }

                _context.GalleryMalls.Update(galleryMallExisting);
                await _context.SaveChangesAsync();

                return Ok(new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Edit successfully",
                    Data = ""
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

        [HttpDelete("delete")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> SoftDelete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    GalleryMall galleryMall = await _context.GalleryMalls.FindAsync(id);

                    if (galleryMall != null)
                    {
                        galleryMall.DeletedAt = DateTime.Now;
                    }
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
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                GalleryMall galleryMall = await _context.GalleryMalls.FindAsync(id);
                if (galleryMall == null)
                {
                    return NotFound();
                }

                galleryMall.DeletedAt = null;

                _context.GalleryMalls.Update(galleryMall);
                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "restore successful",
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
    }
}
