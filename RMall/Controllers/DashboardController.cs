using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using System;

namespace RMall.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public DashboardController(RmallApiContext context)
        {
            _context = context;
        }

        [HttpGet("total-shop")]
        //[Authorize(Roles = "Super Admin, Shopping Center Manager Staff")]
        public async Task<IActionResult> GetShopCount()
        {
            var shopCount = new
            {
                TotalShop = await _context.Shops.Where(m => m.DeletedAt == null).CountAsync(),
            };

            return Ok(shopCount);
        }

        [HttpGet("total-movie")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetMovieCount()
        {
            var movieCount = new
            {
                TotalMovie = await _context.Movies.Where(m => m.DeletedAt == null).CountAsync(),
            };

            return Ok(movieCount);
        }

        [HttpGet("total-cusAndSta")]
        //[Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetUserCount()
        {
            var userCount = new
            {
                TotalUser = await _context.Users.Where(u => u.Role.Equals("User")).CountAsync(),
                TotalStaff = await _context.Users.Where(u => u.Role.Equals("Movie Theater Manager Staff") || u.Role.Equals("Shopping Center Manager Staff")).CountAsync(),
            };

            return Ok(userCount);
        }

        [HttpGet("revenue")]
        //[Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var totalRevenue = new
            {
                TotalRevenue = await _context.Orders.SumAsync(o => o.FinalTotal)
            };

            return Ok(totalRevenue);
        }

        [HttpGet("shows-nowAndUpcoming")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetTotalShowsNow()
        {
            var totalShows = new
            {
                TotalShows = await _context.Shows.Where(m => m.DeletedAt == null).CountAsync(),
                UpcomingShows = await _context.Shows.Where(m => m.DeletedAt == null).CountAsync(s => s.StartDate > DateTime.Now)
            };

            return Ok(totalShows);
        }

        [HttpGet("order-overview")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetOrderOverview()
        {
            var totalShows = new
            {
                DailyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Value.Date == DateTime.Today)
                    .SumAsync(o => o.FinalTotal),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.CreatedAt.Value.Month == DateTime.Today.Month && o.CreatedAt.Value.Year == DateTime.Today.Year)
                    .SumAsync(o => o.FinalTotal)
             };

            return Ok(totalShows);
        }

        [HttpGet("movie/top-10-selling")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetTopSellingMovies()
        {
            try
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var result = await _context.Movies
            .Join(_context.Shows, m => m.Id, s => s.MovieId, (m, s) => new { Movie = m, Show = s })
            .Join(_context.Tickets,
                  ss => new { ShowId = ss.Show.Id, MovieId = ss.Movie.Id },
                  t => new { ShowId = t.Order.ShowId, MovieId = t.Order.Show.MovieId },
                  (ss, t) => new { ss.Movie, ss.Show, Ticket = t })
            .GroupBy(x => new { x.Movie.Id, x.Movie.Title })
            .Select(g => new MovieSaleResult
            {
                MovieId = g.Key.Id,
                MovieTitle = g.Key.Title,
                TicketCount = g.Count(),
            })
            .OrderByDescending(x => x.TicketCount)
            .Take(10)
            .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("shop/top-10-with-traffic")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetTopShop()
        {
            try
            {
                List<Shop> shops = await _context.Shops.Include(s => s.Category).Include(s => s.Floor).Where(s => s.DeletedAt == null).OrderByDescending(s => s.Id).Take(10).ToListAsync();
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
                        address = shop.Address,
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

        [HttpGet("total-product")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetProductCount()
        {
            var productCount = new
            {
                TotalProduct = await _context.Products.Where(m => m.DeletedAt == null).CountAsync(),
            };

            return Ok(productCount);
        }

        [HttpGet("total-promotion")]
        //[Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetPromotionCount()
        {
            var promoCount = new
            {
                TotalPromotion = await _context.Promotions.Where(u => u.DeletedAt == null).CountAsync(),
                TotalPromotionStillValid = await _context.Promotions.Where(u => u.StartDate <= DateTime.Now && u.EndDate >= DateTime.Now).CountAsync(),
            };

            return Ok(promoCount);
        }

        [HttpGet("total-food")]
        //[Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> GetFoodCount()
        {
            var foodCount = new
            {
                TotalPromotion = await _context.Foods.Where(u => u.DeletedAt == null).CountAsync(),
            };

            return Ok(foodCount);
        }
    }
    public class MovieSaleResult
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int TicketCount { get; set; }
    }
}
