using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Products;
using RMall.Service.UploadFiles;

namespace RMall.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IImgService _imgService;
        public ProductController(RmallApiContext context, IImgService imgService)
        {
            _context = context;
            _imgService = imgService;
        }
        [HttpGet("get-all")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                List<Product> products = await _context.Products.Include(p => p.Shop).Where(s => s.DeletedAt == null).OrderByDescending(s => s.Id).ToListAsync();
                List<ProductDTO> result = new List<ProductDTO>();
                foreach (Product product in products)
                {
                    result.Add(new ProductDTO
                    {
                        id = product.Id,
                        name = product.Name,
                        image = product.Image,
                        price = product.Price,
                        description = product.Description,
                        shopId = product.ShopId,
                        shopName = product.Shop.Name,
                        createdAt = product.CreatedAt,
                        updatedAt = product.UpdatedAt,
                        deletedAt = product.DeletedAt,
                    });
                }

                return Ok(result);
            } catch(Exception ex)
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

        [HttpGet("get-by-shop/{slug}")]
        public async Task<IActionResult> GetAllProductByShop(string slug)
        {
            try
            {
                var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Slug.Equals(slug));
                if (shop == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }
                List<Product> products = await _context.Products.Where(s => s.DeletedAt == null && s.ShopId == shop.Id).OrderByDescending(s => s.Id).ToListAsync();
                List<ProductDTO> result = new List<ProductDTO>();
                foreach (Product product in products)
                {
                    result.Add(new ProductDTO
                    {
                        id = product.Id,
                        name = product.Name,
                        image = product.Image,
                        price = product.Price,
                        description = product.Description,
                        shopId = product.ShopId,
                        createdAt = product.CreatedAt,
                        updatedAt = product.UpdatedAt,
                        deletedAt = product.DeletedAt,
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
                List<Product> products = await _context.Products.Where(s => s.DeletedAt != null).OrderByDescending(s => s.DeletedAt).ToListAsync();
                List<ProductDTO> result = new List<ProductDTO>();
                foreach (Product product in products)
                {
                    result.Add(new ProductDTO
                    {
                        id = product.Id,
                        name = product.Name,
                        image = product.Image,
                        price = product.Price,
                        description = product.Description,
                        shopId = product.ShopId,
                        createdAt = product.CreatedAt,
                        updatedAt = product.UpdatedAt,
                        deletedAt = product.DeletedAt,
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
        public async Task<IActionResult> getProductById(int id)
        {
            try
            {
                Product product = await _context.Products.FirstOrDefaultAsync(f => f.Id == id && f.DeletedAt == null);
                if (product != null)
                {
                    return Ok(new ProductDTO
                    {
                        id = product.Id,
                        name= product.Name,
                        price = product.Price,
                        shopId = product.ShopId,
                        description = product.Description,
                        createdAt = product.CreatedAt,
                        updatedAt = product.UpdatedAt,
                        deletedAt = product.DeletedAt

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
        public async Task<IActionResult> CreateProduct([FromForm]CreateProduct model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var imageUrl = await _imgService.UploadImageAsync(model.image, "products");

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
                    Product product = new Product
                    {
                        Name = model.name,
                        Image = imageUrl,
                        Price = model.price,
                        Description = model.description,
                        ShopId = model.shopId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={product.Id}", new ProductDTO
                    {
                        id = product.Id,
                        name = product.Name,
                        image = imageUrl,
                        price = product.Price,
                        description = product.Description,
                        shopId = product.ShopId,
                        createdAt = product.CreatedAt,
                        updatedAt = product.UpdatedAt,
                        deletedAt = product.DeletedAt,
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

        [HttpPut("edit")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> EditProduct([FromForm]EditProduct model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Product existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.id);
                    if (existingProduct == null)
                    {
                        return NotFound(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 404,
                            Message = "Not Found",
                            Data = ""
                        });
                    }

                    Product product = new Product
                    {
                        Id = model.id,
                        Name = model.name,
                        Price = model.price,
                        Description = model.description,
                        ShopId = model.shopId,
                        CreatedAt = existingProduct.CreatedAt,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };

                    if (model.image != null)
                    {
                        string imageUrl = await _imgService.UploadImageAsync(model.image, "products");

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

                        product.Image = imageUrl;
                    }
                    else
                    {
                        product.Image = existingProduct.Image;
                    }

                    _context.Products.Update(product);
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
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> DeleteShop(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Product product = await _context.Products.FindAsync(id);

                    if (product != null)
                    {
                        product.DeletedAt = DateTime.Now;
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
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Product product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.DeletedAt = null;

                _context.Products.Update(product);
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
    }
}
