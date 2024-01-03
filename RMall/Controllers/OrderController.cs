using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.FoodOrder;
using RMall.Models.General;
using RMall.Models.Orders;
using RMall.Models.Tickets;
using System.Net.Sockets;
using System.Security.Claims;

namespace RMall.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RmallApiContext _context;
        public OrderController(RmallApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderAll()
        {
            try
            {
                List<Order> orders = await _context.Orders.OrderByDescending(p => p.Id).ToListAsync();
                List<OrderDTO> result = new List<OrderDTO>();
                foreach (var order in orders)
                {
                    result.Add(new OrderDTO
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        userId = order.UserId,
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
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var randomString = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomString[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomString);
        }

        public static string GetAlphabeticChar(int number)
        {
            // 65 là mã ASCII của 'A'
            char result = (char)(64 + number);
            return result.ToString();
        }


        [HttpGet("get-by-user")]
        [Authorize]
        public async Task<IActionResult> GetOrderByUser()
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

                List<Order> orders = await _context.Orders.Include(p => p.Show).ThenInclude(p => p.Movie).Where(p => p.User.Id == user.Id).OrderByDescending(p => p.Id).ToListAsync();
                List<OrderDTO> result = new List<OrderDTO>();
                foreach (var order in orders)
                {
                    result.Add(new OrderDTO
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        movieTitle = order.Show.Movie.Title,
                        imageMovie = order.Show.Movie.MovieImage,
                        userId = order.UserId,
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

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            try
            {
                Order order = await _context.Orders.Include(o => o.Tickets).ThenInclude(o => o.Seat).Include(o => o.User).Include(o => o.OrderFoods).ThenInclude(o => o.Food).FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
                if (order != null)
                {
                    var show = await _context.Shows.Include(s => s.Movie).FirstOrDefaultAsync(s => s.Id == order.ShowId);

                    var orderDetail = new OrderDetail
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        movieName = show.Movie.Title,
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
                    };

                    List<TicketResponse> tickets = new List<TicketResponse>();  
                    List<OrderFoodResponse> foods = new List<OrderFoodResponse>();

                    foreach (var item in order.Tickets)
                    {
                        var rowChar = GetAlphabeticChar(item.Seat.RowNumber);

                        // Kết hợp RowChar và SeatNumber để tạo seatName
                        var seatName = $"{rowChar}{item.Seat.SeatNumber}";
                        var ticket = new TicketResponse
                        {
                            id = item.Id,
                            code = item.Code,
                            orderId = item.OrderId,
                            startDate = item.StartDate,
                            seatId = item.SeatId,
                            seatName = seatName,
                            price = item.Price,
                            isUsed = item.IsUsed,
                        };
                        tickets.Add(ticket);
                    }
                    orderDetail.tickets = tickets;
                    foreach (var item in order.OrderFoods)
                    {
                        var food = new OrderFoodResponse
                        {
                            id = item.Id,
                            orderId = item.OrderId,
                            foodId = item.FoodId,
                            foodName = item.Food.Name,
                            foodImage = item.Food.Image,
                            price = item.Price,
                            quantity = item.Quantity,
                        };
                        foods.Add(food);
                    }
                    orderDetail.foods = foods;

                    return Ok(orderDetail);
                }
                else
                {
                    return NotFound();
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

        [HttpGet("detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetailForUser(int id)
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

                Order order = await _context.Orders.Include(o => o.Tickets).ThenInclude(o => o.Seat).Include(o => o.User).Include(o => o.OrderFoods).ThenInclude(o => o.Food).FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null && x.UserId == user.Id);
                if (order != null)
                {
                    var show = await _context.Shows.Include(s => s.Movie).FirstOrDefaultAsync(s => s.Id == order.ShowId);

                    var orderDetail = new OrderDetail
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        movieName = show.Movie.Title,
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
                    };

                    List<TicketResponse> tickets = new List<TicketResponse>();
                    List<OrderFoodResponse> foods = new List<OrderFoodResponse>();

                    foreach (var item in order.Tickets)
                    {
                        var rowChar = GetAlphabeticChar(item.Seat.RowNumber);

                        // Kết hợp RowChar và SeatNumber để tạo seatName
                        var seatName = $"{rowChar}{item.Seat.SeatNumber}";
                        var ticket = new TicketResponse
                        {
                            id = item.Id,
                            code = item.Code,
                            orderId = item.OrderId,
                            startDate = item.StartDate,
                            seatId = item.SeatId,
                            seatName = seatName,
                            price = item.Price,
                            isUsed = item.IsUsed,
                        };
                        tickets.Add(ticket);
                    }
                    orderDetail.tickets = tickets;
                    foreach (var item in order.OrderFoods)
                    {
                        var food = new OrderFoodResponse
                        {
                            id = item.Id,
                            orderId = item.OrderId,
                            foodId = item.FoodId,
                            foodName = item.Food.Name,
                            foodImage = item.Food.Image,
                            price = item.Price,
                            quantity = item.Quantity,
                        };
                        foods.Add(food);
                    }
                    orderDetail.foods = foods;

                    return Ok(orderDetail);
                }
                else
                {
                    return NotFound();
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrder model)
        {
            try
            {
                var show = await _context.Shows.FindAsync(model.showId);

                if (show == null)
                {
                    return NotFound(new GeneralServiceResponse
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Not Found",
                        Data = ""
                    });
                }

                Order order = new Order { 
                    OrderCode = model.orderCode,
                    ShowId = model.showId,
                    UserId = model.userId,
                    Total = model.total,
                    DiscountAmount = model.discountAmount,
                    DiscountCode = model.discountCode,
                    FinalTotal = model.finalTotal,
                    Status = 0,
                    PaymentMethod = model.paymentMethod,
                    IsPaid = 0,
                    QrCode = "demo",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in model.tickets)
                {
                    var seat = await _context.Seats.FindAsync(item.seatId);
                    var seatPricing = await _context.SeatPricings.FirstOrDefaultAsync(sp => sp.ShowId == model.showId && sp.SeatTypeId == seat.SeatTypeId);
                    Ticket ticket = new Ticket
                    {
                        Code = GenerateRandomString(10),
                        OrderId = order.Id,
                        StartDate = show.StartDate,
                        SeatId = item.seatId,
                        Price = seatPricing.Price,
                        IsUsed = 0,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    _context.Tickets.Add(ticket);
                    await _context.SaveChangesAsync();
                }

                if(model.foods != null)
                {
                    foreach (var item in model.foods)
                    {
                        var food = await _context.Foods.FindAsync(item.id);
                        OrderFood orderFood = new OrderFood
                        {
                            OrderId = order.Id,
                            FoodId = food.Id,
                            Price = food.Price,
                            Quantity = item.quantity,
                        };

                        food.Quantity = food.Quantity - item.quantity;

                        _context.OrderFoods.Add(orderFood);
                        await _context.SaveChangesAsync();
                    }   
                }

                return Created($"get-by-id?id={order.Id}", new OrderDTO
                {
                    id = order.Id,
                    orderCode = order.OrderCode,
                    showId = order.ShowId,
                    userId = order.UserId,
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
