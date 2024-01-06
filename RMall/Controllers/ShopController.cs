using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Products;
using RMall.Models.Shops;
using RMall.Service.UploadFiles;

namespace RMall.Controllers
{
    [Route("api/shop")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IImgService _imgService;
        public ShopController(RmallApiContext context, IImgService imgService)
        {
            _context = context;
            _imgService = imgService;
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllShop()
        {
            try
            {
                List<Shop> shops = await _context.Shops.Include(s => s.Category).Include(s => s.Floor).Where(s => s.DeletedAt == null).OrderByDescending(s => s.Id).ToListAsync();
                List<ShopDTO> result = new List<ShopDTO>();
                foreach (Shop shop in shops)
                {
                    result.Add(new ShopDTO
                    {
                        id = shop.Id,
                        name = shop.Name,
                        imagePath = shop.ImagePath,
                        slug = shop.Slug,
                        floorId = shop.FloorId,
                        floorName = shop.Floor.FloorNumber,
                        categoryId = shop.CategoryId,
                        categoryName = shop.Category.Name,
                        contactInfo = shop.ContactInfo,
                        hoursOfOperation = shop.HoursOfOperation,
                        description = shop.Description,
                        createdAt = shop.CreatedAt,
                        updatedAt = shop.UpdatedAt,
                        deletedAt = shop.DeletedAt,
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
                List<Shop> shops = await _context.Shops.Include(s => s.Category).Include(s => s.Floor).Where(s => s.DeletedAt != null).OrderByDescending(s => s.DeletedAt).ToListAsync();
                List<ShopDTO> result = new List<ShopDTO>();
                foreach (Shop shop in shops)
                {
                    result.Add(new ShopDTO
                    {
                        id = shop.Id,
                        name = shop.Name,
                        imagePath = shop.ImagePath,
                        slug = shop.Slug,
                        floorId = shop.FloorId,
                        floorName = shop.Floor.FloorNumber,
                        categoryId = shop.CategoryId,
                        categoryName = shop.Category.Name,
                        contactInfo = shop.ContactInfo,
                        hoursOfOperation = shop.HoursOfOperation,
                        description = shop.Description,
                        createdAt = shop.CreatedAt,
                        updatedAt = shop.UpdatedAt,
                        deletedAt = shop.DeletedAt,
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

        [HttpGet("get-all-by-floor/{floor_name}")]
        public async Task<IActionResult> GetAllShopBylFoor(string floor_name)
        {
            try
            {
                var floor = await _context.Floors.FirstOrDefaultAsync(f => f.FloorNumber.Equals(floor_name));
                if (floor == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }

                List<Shop> shops = await _context.Shops.Include(s => s.Category).Include(s => s.Floor).Where(s => s.DeletedAt == null && s.FloorId == floor.Id).OrderByDescending(s => s.Id).ToListAsync();
                List<ShopDTO> result = new List<ShopDTO>();
                foreach (Shop shop in shops)
                {
                    result.Add(new ShopDTO
                    {
                        id = shop.Id,
                        name = shop.Name,
                        imagePath = shop.ImagePath,
                        slug = shop.Slug,
                        floorId = shop.FloorId,
                        floorName = shop.Floor.FloorNumber,
                        categoryId = shop.CategoryId,
                        categoryName = shop.Category.Name,
                        contactInfo = shop.ContactInfo,
                        hoursOfOperation = shop.HoursOfOperation,
                        description = shop.Description,
                        createdAt = shop.CreatedAt,
                        updatedAt = shop.UpdatedAt,
                        deletedAt = shop.DeletedAt,
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

        [HttpGet("detail/{slug}")]
        public async Task<IActionResult> DetailShop(string slug)
        {
            try
            {
                Shop shop = await _context.Shops.Include(s => s.Floor).Include(s => s.Category).Include(s => s.Products).FirstOrDefaultAsync(x => x.Slug == slug);
                if (shop == null)
                {
                    return NotFound();
                }
                var shopDetail = new ShopDetail
                {
                    id = shop.Id,
                    name = shop.Name,
                    imagePath = shop.ImagePath,
                    slug = shop.Slug,
                    floorId = shop.FloorId,
                    floorName = shop.Floor.FloorNumber,
                    categoryId = shop.CategoryId,
                    categoryName = shop.Category.Name,
                    contactInfo = shop.ContactInfo,
                    hoursOfOperation = shop.HoursOfOperation,
                    description = shop.Description,
                    createdAt = shop.CreatedAt,
                    updatedAt = shop.UpdatedAt,
                    deletedAt = shop.DeletedAt,
                };

                List<ProductResponse> products = new List<ProductResponse>();

                foreach (var item in shop.Products)
                {
                    products.Add(new ProductResponse
                    {
                        id = item.Id,
                        name = item.Name,
                        price = item.Price,
                        image = item.Image,
                        description = item.Description,
                    });
                }

                shopDetail.products = products;

                return Ok(shopDetail);
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

        [HttpPost("create")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> CreateShop([FromForm]CreateShop model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool nameExists = await _context.Shops.AnyAsync(c => c.Name == model.name);

                    if (nameExists)
                    {
                        // Nếu name đã tồn tại, trả về BadRequest hoặc thông báo lỗi tương tự
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Shop name already exists",
                            Data = ""
                        });
                    }

                    var imageUrl = await _imgService.UploadImageAsync(model.imagePath, "shops");

                    if (imageUrl == null)
                    {
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Please provide a image.",
                            Data = ""
                        });
                    }

                    Shop shop = new Shop
                    {
                        Name = model.name,
                        Slug = model.name.ToLower().Replace(" ", "-"),
                        ImagePath = imageUrl,
                        FloorId = model.floorId,
                        CategoryId = model.categoryId,
                        ContactInfo = model.contactInfo,
                        HoursOfOperation = model.hoursOfOperation,
                        Description = model.description,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    _context.Shops.Add(shop);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={shop.Id}", new ShopDTO
                    {
                        id = shop.Id,
                        name = shop.Name,
                        slug = shop.Slug,
                        imagePath = imageUrl,
                        floorId = shop.FloorId,
                        categoryId = shop.CategoryId,
                        createdAt = shop.CreatedAt,
                        updatedAt = shop.UpdatedAt,
                        deletedAt = shop.DeletedAt,
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

        [HttpPut("edit")]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditShop([FromForm] EditShop model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Shop existingShop = await _context.Shops.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.id);
                    if (existingShop == null)
                    {
                        return NotFound(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 404,
                            Message = "Not Found",
                            Data = ""
                        });
                    }
                    // Kiểm tra xem name đã tồn tại trong cơ sở dữ liệu hay chưa (trừ trường hợp cập nhật cùng tên)
                    bool nameExists = await _context.Shops.AnyAsync(c => c.Name == model.name && c.Id != model.id);

                    if (nameExists)
                    {
                        // Nếu name đã tồn tại, trả về BadRequest hoặc thông báo lỗi tương tự
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Shop name already exists",
                            Data = ""
                        });
                    }

                    Shop shop = new Shop
                    {
                        Id = model.id,
                        Name = model.name,
                        Slug = model.name.ToLower().Replace(" ", "-"),
                        FloorId = model.floorId,
                        CategoryId = model.categoryId,
                        ContactInfo = model.contactInfo,
                        HoursOfOperation = model.hoursOfOperation,
                        Description = model.description,
                        CreatedAt = existingShop.CreatedAt,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };
                    if (model.imagePath != null)
                    {
                        string imageUrl = await _imgService.UploadImageAsync(model.imagePath, "shops");

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

                        shop.ImagePath = imageUrl;
                    }
                    else
                    {
                        shop.ImagePath = existingShop.ImagePath;
                    }

                    _context.Shops.Update(shop);
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

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteShop(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Shop shop = await _context.Shops.FindAsync(id);

                    if (shop != null)
                    {
                        shop.DeletedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Soft delete successful",
                    Data = null
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

        [HttpPut]
        [Route("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Shop shop = await _context.Shops.FindAsync(id);
                if (shop == null)
                {
                    return NotFound();
                }

                shop.DeletedAt = null;

                _context.Shops.Update(shop);
                await _context.SaveChangesAsync();

                var response = new GeneralServiceResponse
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "restore successful",
                    Data = null
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

        [HttpGet("/api/category/get-all")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                List<Category> categories = await _context.Categories.ToListAsync();
                List<CategoryDTO> result = new List<CategoryDTO>();
                foreach (Category category in categories)
                {
                    result.Add(new CategoryDTO
                    {
                        id = category.Id,
                        name = category.Name,
                        slug = category.Slug,
                        createdAt = category.CreatedAt,
                        updatedAt = category.UpdatedAt,
                        deletedAt = category.DeletedAt,
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

        [HttpGet("/api/floor/get-all")]
        public async Task<IActionResult> GetAllFloor()
        {
            try
            {
                List<Floor> floors = await _context.Floors.ToListAsync();
                List<FloorDTO> result = new List<FloorDTO>();
                foreach (Floor floor in floors)
                {
                    result.Add(new FloorDTO
                    {
                        id = floor.Id,
                        floorNumber = floor.FloorNumber,
                        createdAt = floor.CreatedAt,
                        updatedAt = floor.UpdatedAt,
                        deletedAt = floor.DeletedAt,
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
