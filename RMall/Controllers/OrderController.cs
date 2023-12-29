using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMall.DTOs;
using RMall.Entities;
using RMall.Models.General;
using RMall.Models.Orders;
using System.Net.Sockets;

namespace RMall.Controllers
{
    [Route("api/[controller]")]
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
                List<Order> orders = await _context.Orders.Where(p => p.DeletedAt == null).OrderByDescending(p => p.Id).ToListAsync();
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
