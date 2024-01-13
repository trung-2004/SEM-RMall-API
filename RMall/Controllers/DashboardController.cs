using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.SeatPricings;
using RMall.Models.Seats;
using System;
using System.Net.WebSockets;
using System.Text;

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

        [HttpGet("total-show-today")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetShowToDayCount()
        {
            DateTime today = DateTime.Now.Date; // Lấy ngày hiện tại (bỏ qua giờ, phút, giây)

            var showCount = await _context.Shows
                .Where(s => s.DeletedAt == null && s.StartDate.Date == today)
                .CountAsync();

            var result = new
            {
                TotalShowToday = showCount,
            };

            return Ok(result);
        }

        [HttpGet("list-show-today")]
        public async Task<IActionResult> GetListShowToDay()
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                List<Show> shows = await _context.Shows.Include(s => s.Movie).Include(s => s.Room).Include(m => m.SeatPricings).ThenInclude(m => m.SeatType).Where(m => m.DeletedAt == null && m.StartDate.Date == today).OrderByDescending(m => m.Id).ToListAsync();

                List<ShowDTO> result = new List<ShowDTO>();
                foreach (Show s in shows)
                {
                    var showDto = new ShowDTO
                    {
                        id = s.Id,
                        movieId = s.MovieId,
                        roomId = s.RoomId,
                        movieName = s.Movie.Title,
                        roomName = s.Room.Name,
                        startDate = s.StartDate,
                        showCode = s.ShowCode,
                        language = s.Language,
                        createdAt = s.CreatedAt,
                        updatedAt = s.UpdatedAt,
                        deletedAt = s.DeletedAt,
                    };

                    var seatPricings = new List<SeatPricingResponse>();

                    foreach (var item in s.SeatPricings)
                    {
                        var seatPricing = new SeatPricingResponse
                        {
                            id = item.Id,
                            showId = item.ShowId,
                            seatTypeId = item.SeatTypeId,
                            seatTypeName = item.SeatType.Name,
                            price = item.Price,
                        };
                        seatPricings.Add(seatPricing);
                        showDto.seatPricings = seatPricings;
                    }

                    result.Add(showDto);
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

        [HttpGet("total-order-today")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetOrderToDayCount()
        {
            DateTime today = DateTime.Now.Date; // Lấy ngày hiện tại (bỏ qua giờ, phút, giây)

            var showCount = await _context.Orders
                .Where(s => s.DeletedAt == null && s.CreatedAt.Value.Date == today)
                .CountAsync();

            var result = new
            {
                TotalShowToday = showCount,
            };

            return Ok(result);
        }

        [HttpGet("list-order-today")]
        public async Task<IActionResult> GetListOrderToDay()
        {
            try
            {
                DateTime today = DateTime.Now.Date;
                List<Order> orders = await _context.Orders.Include(o => o.User).Include(o => o.Show).ThenInclude(o => o.Movie).Where(o => o.CreatedAt.Value.Date == today).OrderByDescending(p => p.CreatedAt).ToListAsync();
                List<OrderDTO> result = new List<OrderDTO>();
                foreach (var order in orders)
                {
                    result.Add(new OrderDTO
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        movieTitle = order.Show.Movie.Title,
                        imageMovie = order.Show.Movie.MovieImage,
                        showId = order.ShowId,
                        userId = order.UserId,
                        userName = order.User.Fullname,
                        total = order.Total,
                        discountAmount = order.DiscountAmount,
                        discountCode = order.DiscountCode,
                        finalTotal = order.FinalTotal,
                        status = order.Status,
                        paymentMethod = order.PaymentMethod,
                        isPaid = order.IsPaid,
                        createdAt = order.CreatedAt,
                        updatedAt = order.UpdatedAt,
                        deletedAt = order.DeletedAt,
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
            .GroupBy(x => new { x.Movie.Id, x.Movie.Title, x.Movie.MovieImage })
            .Select(g => new MovieSaleResult
            {
                MovieId = g.Key.Id,
                MovieTitle = g.Key.Title,
                MovieImage = g.Key.MovieImage,
                TicketCount = g.Count(),
                Money = (int)g.Sum(t => t.Ticket.Price)
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

        [HttpGet("revenue/weekly")]
        //[Authorize(Roles = "Super Admin")]
        public IActionResult GetWeeklySales()
        {
            // Get the first day of the current week (assuming Sunday as the first day)
            DateTime startDate = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);

            // Create a list of all days in the week
            List<DateTime> allDaysOfWeek = Enumerable.Range(0, 7)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            // Implement logic to retrieve daily sales for the current week
            var weeklySales = allDaysOfWeek
                .GroupJoin(_context.Orders,
                    date => date.Date,
                    order => order.CreatedAt.Value.Date,
                    (date, orders) => new
                    {
                        Date = date,
                        TotalSales = orders.Sum(o => o.Total)
                    })
                .Select(result => new
                {
                    Date = result.Date,
                    TotalSales = result.TotalSales
                })
                .ToList();

            return Ok(weeklySales);
        }

        [HttpGet("revenue/monthly/{year}")]
        //[Authorize(Roles = "Super Admin")]
        public IActionResult GetMonthlySales(int year)
        {
            // Validate the input year
            if (year <= 0)
            {
                return BadRequest("Invalid year parameter.");
            }

            // Get the first day of the selected year
            DateTime startDate = new DateTime(year, 1, 1);

            // Create a list of all months in the selected year
            List<DateTime> allMonthsOfYear = Enumerable.Range(0, 12)
                .Select(offset => startDate.AddMonths(offset))
                .ToList();

            // Implement logic to retrieve monthly sales for the selected year
            var monthlySales = allMonthsOfYear
                .GroupJoin(_context.Orders,
                    date => new { Year = date.Year, Month = date.Month },
                    order => new { Year = order.CreatedAt.Value.Year, Month = order.CreatedAt.Value.Month },
                    (date, orders) => new
                    {
                        Year = date.Year,
                        Month = date.Month,
                        TotalSales = orders.Sum(o => o.Total)
                    })
                .OrderBy(result => result.Year)
                .ThenBy(result => result.Month)
                .Select(result => new
                {
                    Year = result.Year,
                    Month = result.Month,
                    TotalSales = result.TotalSales
                })
                .ToList();

            return Ok(monthlySales);
        }

        [HttpGet("revenue/yearly")]
        //[Authorize(Roles = "Super Admin")]
        public IActionResult GetYearlySales()
        {
            int currentYear = DateTime.UtcNow.Year;

            // Calculate the start year for the past 5 years
            int startYear = currentYear - 4;

            // Create a list of all years in the past 5 years
            List<int> allYears = Enumerable.Range(startYear, 5).ToList();

            // Implement logic to retrieve yearly sales for the past 5 years
            var yearlySales = allYears
                .GroupJoin(_context.Orders,
                    year => year,
                    order => order.CreatedAt.Value.Year,
                    (year, orders) => new
                    {
                        Year = year,
                        TotalSales = orders.Sum(o => o.Total)
                    })
                .OrderBy(result => result.Year)
                .ToList();

            return Ok(yearlySales);
        }

        [HttpGet("shows/performance-chart")]
        public async Task<IActionResult> GetShowsPerformanceChart()
        {
            try
            {
                var performanceData = await _context.Shows
            .OrderBy(s => s.StartDate)
            .Select(s => new
            {
                Date = s.StartDate.Date,
                TotalTicketsSold = s.Orders.SelectMany(o => o.Tickets).Count(),
                TotalRevenue = s.Orders.SelectMany(o => o.Tickets).Sum(t => t.Price)
            })
            .ToListAsync();

                return Ok(performanceData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("ws")]
        public async Task Get(string ShowCode)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var random = new Random();
                while(webSocket.State == WebSocketState.Open)
                {
                    var seats = await GetAllSeatByShow(ShowCode);

                    // Chuyển đổi danh sách ghế thành chuỗi JSON
                    var seatsJson = JsonConvert.SerializeObject(seats);

                    // Gửi dữ liệu ghế đến người dùng qua WebSocket
                    var buffer = Encoding.UTF8.GetBytes(seatsJson);
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    _context.ChangeTracker.Clear();

                    await Task.Delay(5000);
                }
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "connection close by the server", CancellationToken.None);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task<IActionResult> GetAllSeatByShow(string ShowCode)
        {
            try
            {
                var show = await _context.Shows.Include(s => s.Orders).ThenInclude(s => s.Tickets).FirstOrDefaultAsync(s => s.ShowCode.Equals(ShowCode) && s.DeletedAt == null);
                
                List<Seat> seats = await _context.Seats
                    .Where(s => s.RoomId == show.RoomId)
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .ToListAsync();

                List<int> seatsBooked = show.Orders
                    .SelectMany(o => o.Tickets.Select(t => t.SeatId))
                    .ToList();

                var seatPricings = await _context.SeatPricings
                    .Where(sp => sp.ShowId == show.Id)
                    .ToListAsync();

                List<SeatResponse> result = seats.Select(seat =>
                {
                    var seatPricing = seatPricings.FirstOrDefault(sp => sp.SeatTypeId == seat.SeatTypeId);
                    decimal price = seatPricing != null ? seatPricing.Price : 0;
                    var seatReservation = _context.SeatReservations.FirstOrDefault(s => s.ShowId == show.Id && s.SeatId == seat.Id);
                    if (seatReservation == null)
                    {
                        return new SeatResponse
                        {
                            id = seat.Id,
                            roomId = seat.RoomId,
                            seatTypeId = seat.SeatTypeId,
                            rowNumber = seat.RowNumber,
                            seatNumber = seat.SeatNumber,
                            createdAt = seat.CreatedAt,
                            updatedAt = seat.UpdatedAt,
                            deletedAt = seat.DeletedAt,
                            isBooked = seatsBooked.Contains(seat.Id),
                            isReserved = false,
                            price = price,
                        };
                    }
                    else if (seatReservation.ReservationExpiresAt == null)
                    {
                        return new SeatResponse
                        {
                            id = seat.Id,
                            roomId = seat.RoomId,
                            seatTypeId = seat.SeatTypeId,
                            rowNumber = seat.RowNumber,
                            seatNumber = seat.SeatNumber,
                            createdAt = seat.CreatedAt,
                            updatedAt = seat.UpdatedAt,
                            deletedAt = seat.DeletedAt,
                            isBooked = seatsBooked.Contains(seat.Id),
                            isReserved = false,
                            price = price,
                        };
                    }
                    else if (DateTime.Now > seatReservation.ReservationExpiresAt)
                    {
                        return new SeatResponse
                        {
                            id = seat.Id,
                            roomId = seat.RoomId,
                            seatTypeId = seat.SeatTypeId,
                            rowNumber = seat.RowNumber,
                            seatNumber = seat.SeatNumber,
                            createdAt = seat.CreatedAt,
                            updatedAt = seat.UpdatedAt,
                            deletedAt = seat.DeletedAt,
                            isBooked = seatsBooked.Contains(seat.Id),
                            isReserved = false,
                            price = price,
                        };
                    }
                    else
                    {
                        return new SeatResponse
                        {
                            id = seat.Id,
                            roomId = seat.RoomId,
                            seatTypeId = seat.SeatTypeId,
                            rowNumber = seat.RowNumber,
                            seatNumber = seat.SeatNumber,
                            createdAt = seat.CreatedAt,
                            updatedAt = seat.UpdatedAt,
                            deletedAt = seat.DeletedAt,
                            isBooked = seatsBooked.Contains(seat.Id),
                            isReserved = true,
                            price = price,
                        };
                    }
                }).ToList();

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
    public class MovieSaleResult
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string MovieImage { get; set; }
        public int TicketCount { get; set; }
        public int Money { get; set; }
    }
}
