using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Helper.Render;
using RMall.Models.General;
using RMall.Models.Promotions;
using RMall.Models.UserPromotions;
using System.Security.Claims;

namespace RMall.Controllers
{
    [Route("api/promotion")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public PromotionController(RmallApiContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPromotion()
        {
            try
            {
                List<Promotion> promotions = await _context.Promotions.Where(p => p.DeletedAt == null).OrderByDescending(p => p.Id).ToListAsync();
                List<PromotionDTO> result = new List<PromotionDTO>();
                foreach (var item in promotions)
                {
                    result.Add(new PromotionDTO
                    {
                        id = item.Id,
                        name = item.Name,
                        slug = item.Slug,
                        startDate = item.StartDate,
                        endDate = item.EndDate,
                        discountPercentage = item.DiscountPercentage,
                        limit = item.Limit,
                        couponCode = item.CouponCode,
                        minPurchaseAmount = item.MinPurchaseAmount,
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

        [HttpGet("get-all-for-user")]
        public async Task<IActionResult> GetAllPromotionForUser()
        {
            try
            {
                List<Promotion> promotions = await _context.Promotions.Where(p => p.DeletedAt == null && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now).OrderByDescending(p => p.Id).ToListAsync();
                List<PromotionDTO> result = new List<PromotionDTO>();
                foreach (var item in promotions)
                {
                    result.Add(new PromotionDTO
                    {
                        id = item.Id,
                        name = item.Name,
                        slug = item.Slug,
                        startDate = item.StartDate,
                        endDate = item.EndDate,
                        discountPercentage = item.DiscountPercentage,
                        limit = item.Limit,
                        couponCode = item.CouponCode,
                        minPurchaseAmount = item.MinPurchaseAmount,
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
        public async Task<IActionResult> GetPromotionById(int id)
        {
            try
            {
                Promotion promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
                if(promotion != null)
                {
                    return Ok(new PromotionDTO
                    {
                        id = promotion.Id,
                        name = promotion.Name,
                        slug = promotion.Slug,
                        startDate = promotion.StartDate,
                        endDate = promotion.EndDate,
                        discountPercentage = promotion.DiscountPercentage,
                        limit = promotion.Limit,
                        couponCode = promotion.CouponCode,
                        minPurchaseAmount = promotion.MinPurchaseAmount,
                        createdAt = promotion.CreatedAt,
                        updatedAt = promotion.UpdatedAt,
                        deletedAt = promotion.DeletedAt,
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
                List<Promotion> promotions = await _context.Promotions.Where(p => p.DeletedAt != null).OrderByDescending(p => p.DeletedAt).ToListAsync();
                List<PromotionDTO> result = new List<PromotionDTO>();
                foreach (var item in promotions)
                {
                    result.Add(new PromotionDTO
                    {
                        id = item.Id,
                        name = item.Name,
                        slug = item.Slug,
                        startDate = item.StartDate,
                        endDate = item.EndDate,
                        discountPercentage = item.DiscountPercentage,
                        limit = item.Limit,
                        couponCode = item.CouponCode,
                        minPurchaseAmount = item.MinPurchaseAmount,
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

        [HttpPost("create")]
        public async Task<IActionResult> CreatePromotion(CreatePromotion model)
        {
            try
            {
                var couponCode = GenerateRandom.GenerateRandomString(8);

                var couponCodeExisting = await _context.Promotions.FirstOrDefaultAsync(c => c.CouponCode.Equals(couponCode));

                if (couponCodeExisting != null)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Coupon Code already exists",
                        Data = ""
                    });
                }

                Promotion promotion = new Promotion
                {
                    Name = model.name,
                    Slug = model.name.ToLower().Replace(" ", "-"),
                    StartDate = model.startDate,
                    EndDate = model.endDate,
                    DiscountPercentage = model.discountPercentage,
                    Limit = model.limit,
                    CouponCode = couponCode,
                    MinPurchaseAmount = model.minPurchaseAmount,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null
                };

                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();

                return Created($"get-by-id?id={promotion.Id}", new PromotionDTO
                {
                    id = promotion.Id,
                    name = promotion.Name,
                    slug = promotion.Slug,
                    startDate = promotion.StartDate,
                    endDate = promotion.EndDate,
                    discountPercentage = promotion.DiscountPercentage,
                    limit = promotion.Limit,
                    couponCode = promotion.CouponCode,
                    minPurchaseAmount = promotion.MinPurchaseAmount,
                    createdAt = promotion.CreatedAt,
                    updatedAt = promotion.UpdatedAt,
                    deletedAt = promotion.DeletedAt,
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

        [HttpPut("edit")]
        public async Task<IActionResult> EditPromotion(EditPromotion model)
        {
            try
            {
                Promotion promotionExisting = await _context.Promotions.FindAsync(model.id);
                if (promotionExisting == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }

                var couponCodeExisting = await _context.Promotions.AnyAsync(c => c.CouponCode.Equals(model.couponCode) && c.Id != model.id);

                if (couponCodeExisting)
                {
                    return BadRequest(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Coupon Code already exists",
                        Data = ""
                    });
                }

                Promotion promotion = new Promotion
                {
                    Id = model.id,
                    Name = model.name,
                    Slug = model.name.ToLower().Replace(" ", "-"),
                    StartDate = model.startDate,
                    EndDate = model.endDate,
                    DiscountPercentage = model.discountPercentage,
                    Limit = model.limit,
                    CouponCode = model.couponCode,
                    MinPurchaseAmount = model.minPurchaseAmount,
                    CreatedAt = promotionExisting.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                };

                _context.Promotions.Update(promotion);
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

        [HttpDelete("delete")]
        public async Task<IActionResult> SoftDelete(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Promotion promotion = await _context.Promotions.FindAsync(id);
                    if (promotion != null)
                    {
                        promotion.DeletedAt = DateTime.Now;
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

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                Promotion promotion = await _context.Promotions.FindAsync(id);
                if(promotion == null)
                {
                    return NotFound();
                }

                promotion.DeletedAt = null;

                _context.Promotions.Update(promotion);
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

        [HttpGet("get-promotionUser")]
        public async Task<IActionResult> GetAllPromotionUser()
        {
            try
            {
                List<UserPromotion> userPromotions = await _context.UserPromotions.Include(up => up.User).Include(up => up.Promotion).OrderByDescending(up => up.Id).ToListAsync();
                List<UserPromotionDTO> result = new List<UserPromotionDTO>();
                foreach (var item in userPromotions)
                {
                    result.Add(new UserPromotionDTO
                    {
                        id = item.Id,
                        userId = item.UserId,
                        nameUser = item.User.Fullname,
                        promotionId = item.PromotionId,
                        promotionName = item.Promotion.Name,
                        isUsed = item.IsUsed,
                        usedAt = item.UsedAt,
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

        [HttpPost("create-promotion-user")]
        public async Task<IActionResult> CreateUserPromotion(CreateUserPromotion model)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var userClaims = identity.Claims;
                    var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                    if (user == null)
                    {
                        return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                    }

                    Promotion promotion = await _context.Promotions.FirstAsync(p => p.Id == model.promotionId);
                    if (promotion == null)
                    {
                        return NotFound(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 404,
                            Message = "Not Found",
                            Data = ""
                        });
                    }

                    var userPromotionExisting = await _context.UserPromotions.FirstOrDefaultAsync(up => up.PromotionId == model.promotionId && up.UserId == user.Id);
                    if(userPromotionExisting != null)
                    {
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "This promotion you already have",
                            Data = ""
                        });
                    }

                    var userPromotionExistings = await _context.UserPromotions.Where(up => up.PromotionId == model.promotionId).ToListAsync();

                    if (userPromotionExistings.Count > promotion.Limit)
                    {
                        return BadRequest(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 400,
                            Message = "Promotion has ended",
                            Data = ""
                        });
                    }

                    UserPromotion userPromotion = new UserPromotion
                    {
                        UserId = user.Id,
                        PromotionId = model.promotionId,
                        IsUsed = 0,
                        UsedAt = null,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null
                    };
                    _context.UserPromotions.Add(userPromotion);
                    await _context.SaveChangesAsync();

                    return Created($"get-by-id?id={userPromotion.Id}", new UserPromotionDTO
                    {
                        id = userPromotion.Id,
                        userId = userPromotion.UserId,
                        promotionId = userPromotion.PromotionId,
                        promotionName = null,
                        nameUser = null,
                        isUsed = userPromotion.IsUsed,
                        usedAt = userPromotion.UsedAt,
                        createdAt = userPromotion.CreatedAt,
                        updatedAt = userPromotion.UpdatedAt,
                        deletedAt = userPromotion.DeletedAt,
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

        [HttpGet("get-promotion-by-user")]
        [Authorize]
        public async Task<IActionResult> GetPromotionByUser()
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

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                List<UserPromotion> userPromotions = await _context.UserPromotions.Include(up => up.Promotion).Where(up => up.UserId == user.Id && up.IsUsed == 0 && up.Promotion.EndDate >= DateTime.Now).OrderByDescending(up => up.Id).ToListAsync();
                List<UserPromotionDTO> result = new List<UserPromotionDTO>();
                foreach (var item in userPromotions)
                {
                    result.Add(new UserPromotionDTO
                    {
                        id = item.Id,
                        userId = item.UserId,
                        promotionId = item.PromotionId,
                        promotionName = item.Promotion.Name,
                        promotionCode = item.Promotion.CouponCode,
                        startDate = item.Promotion.StartDate,
                        endDate = item.Promotion.EndDate,
                        discountPercentage = item.Promotion.DiscountPercentage,
                        isUsed = item.IsUsed,
                        usedAt = item.UsedAt,
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

        [HttpGet("checkDiscount")]
        [Authorize]
        public async Task<IActionResult> CheckDiscount(string couponCode, decimal total)
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

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                var promotion = await _context.Promotions
                    .Where(promotion => promotion.CouponCode == couponCode)
                    .FirstOrDefaultAsync();

                if (promotion != null && promotion.StartDate <= DateTime.Now && promotion.EndDate >= DateTime.Now)
                {
                    var userPromotion = await _context.UserPromotions
                        .Where(up => up.UserId == user.Id && up.PromotionId == promotion.Id)
                        .FirstOrDefaultAsync();

                    if (userPromotion != null)
                    {
                        if(userPromotion.IsUsed == 0)
                        {
                            if (total >= promotion.MinPurchaseAmount)
                            {
                                return Ok(new GeneralServiceResponse
                                {
                                    Success = true,
                                    StatusCode = 200,
                                    Message = "Valid promo code",
                                    Data = true
                                });
                            }
                            else
                            {
                                return Ok(new GeneralServiceResponse
                                {
                                    Success = false,
                                    StatusCode = 200,
                                    Message = "The order does not meet promotion conditions",
                                    Data = false
                                });
                            }
                        }
                        else
                        {
                            return Ok(new GeneralServiceResponse
                            {
                                Success = false,
                                StatusCode = 200,
                                Message = "User has already used this promotion.",
                                Data = false
                            });
                        }
                    }
                    else
                    {
                        return Ok(new GeneralServiceResponse
                        {
                            Success = false,
                            StatusCode = 200,
                            Message = "Invalid or expired promotion.",
                            Data = false
                        });
                    }
                }
                else
                {
                    return Ok(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 200,
                        Message = "Invalid or expired promotion.",
                        Data = false
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

        [HttpPost("discounts")]
        [Authorize]
        public async Task<IActionResult> Discounts(string promotionCode, decimal total)
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

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(userId));

                if (user == null)
                {
                    return Unauthorized(new GeneralServiceResponse { Success = false, StatusCode = 401, Message = "Not Authorized", Data = "" });
                }

                var promotion = await _context.Promotions.FirstOrDefaultAsync(p => p.CouponCode.Equals(promotionCode));

                var finalTotal = total - total * ((decimal)promotion.DiscountPercentage / 100);

                var userPromotion = await _context.UserPromotions.FirstOrDefaultAsync(up => up.PromotionId == promotion.Id && up.UserId == user.Id);

                userPromotion.IsUsed = 1;
                userPromotion.UsedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new GeneralServiceResponse
                {
                    Success = false,
                    StatusCode = 200,
                    Message = "successful discount.",
                    Data = finalTotal
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
    }
}
