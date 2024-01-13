using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using RMall.DTOs;
using RMall.Entities;
using RMall.Helper.Email;
using RMall.Helper.Render;
using RMall.Models.FoodOrder;
using RMall.Models.General;
using RMall.Models.Orders;
using RMall.Models.Tickets;
using RMall.Service.Email;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RMall.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly RmallApiContext _context;
        private readonly IEmailService _emailService;
        public OrderController(RmallApiContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [HttpGet]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]

        public async Task<IActionResult> GetOrderAll()
        {
            try
            {
                List<Order> orders = await _context.Orders.Include(o => o.User).Include(o => o.Show).ThenInclude(o => o.Movie).OrderByDescending(p => p.Id).ToListAsync();
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

        [HttpGet("get-by-id/{code_order}")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetOrderDetail(string code_order)
        {
            try
            {
                Order order = await _context.Orders.Include(o => o.Tickets).ThenInclude(o => o.Seat).Include(o => o.User).Include(o => o.OrderFoods).ThenInclude(o => o.Food).FirstOrDefaultAsync(x => x.OrderCode.Equals(code_order) && x.DeletedAt == null);
                if (order != null)
                {
                    var show = await _context.Shows.Include(s => s.Movie).Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == order.ShowId);

                    var orderDetail = new OrderDetail
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        startDate = show.StartDate,
                        roomName = show.Room.Name,
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
                        qrCode = order.QrCode,
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

        [HttpGet("detail/{code_order}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetailForUser(string code_order)
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

                Order order = await _context.Orders.Include(o => o.Tickets).ThenInclude(o => o.Seat).Include(o => o.User).Include(o => o.OrderFoods).ThenInclude(o => o.Food).FirstOrDefaultAsync(x => x.OrderCode.Equals(code_order) && x.DeletedAt == null && x.UserId == user.Id);
                if (order != null)
                {
                    var show = await _context.Shows.Include(s => s.Movie).Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == order.ShowId);

                    var orderDetail = new OrderDetail
                    {
                        id = order.Id,
                        orderCode = order.OrderCode,
                        showId = order.ShowId,
                        startDate = show.StartDate,
                        roomName = show.Room.Name,
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
                        qrCode = order.QrCode,
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
        [Authorize]
        public async Task<IActionResult> CreateOrder(CreateOrder model)
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

                var show = await _context.Shows.Include(s => s.Movie).Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == model.showId);

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
                    OrderCode = GenerateRandom.GenerateRandomString(8),
                    ShowId = model.showId,
                    UserId = user.Id,
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

                // Tạo mã QR code từ OrderCode của order
                var qrCodeContent = order.OrderCode;
                var qrCodeImage = GenerateQrCodePixelData(qrCodeContent);

                // Lưu hình ảnh QR code vào thư mục
                string qrCodeFileName = $"qrcode_{order.Id}.png";
                SaveQrCodeImage(qrCodeImage, qrCodeFileName);

                // Lưu tên hình ảnh QR code vào order
                order.QrCode = GenerateQrCodeImageUrl(qrCodeFileName);

                await _context.SaveChangesAsync();

                foreach (var item in model.tickets)
                {
                    var seat = await _context.Seats.FindAsync(item.seatId);
                    var seatPricing = await _context.SeatPricings.FirstOrDefaultAsync(sp => sp.ShowId == model.showId && sp.SeatTypeId == seat.SeatTypeId);
                    Ticket ticket = new Ticket
                    {
                        Code = GenerateRandom.GenerateRandomString(10),
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

                var orderDetail = new
                {
                    MovieName = show.Movie.Title,
                    OrderCode = order.OrderCode,
                    QRCode = order.QrCode,
                    Screen = show.Room.Name,
                    StartDate = show.StartDate,
                    CreateAt = order.CreatedAt,
                    Tickets = tickets,
                    Foods = foods,
                    Total = order.Total,
                    FinalTotal = order.FinalTotal,
                    PaymentMethod = order.PaymentMethod,
                };

                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = user.Email;
                mailrequest.Subject = "R Ticket: Successful Transaction";
                mailrequest.Body = EmailContentOrder.GetHtmlcontentOrder(orderDetail.MovieName, order.QrCode, orderDetail.OrderCode, orderDetail.Screen, orderDetail.StartDate, orderDetail.StartDate, orderDetail.Total, orderDetail.FinalTotal, orderDetail.PaymentMethod, orderDetail.Tickets, orderDetail.Foods);
                await _emailService.SendEmailAsync(mailrequest);

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

        private byte[] GenerateQrCodePixelData(string content)
        {
            byte[] QRCode = new byte[0];
            if (!string.IsNullOrEmpty(content))
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qRCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
                QRCode = bitmap.GetGraphic(20);
            }
            return QRCode;
        }

        private void SaveQrCodeImage(byte[] pixelData, string fileName)
        {
            string uploadDirectory = GetUploadDirectory();
            string filePath = Path.Combine(uploadDirectory, fileName);

            Directory.CreateDirectory(uploadDirectory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Write(pixelData, 0, pixelData.Length);
            }
        }

        private string GetUploadDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "orders");
        }

        private string GenerateQrCodeImageUrl(string fileName)
        {
            string baseUrl = "https://localhost:7220"; // Thay thế bằng URL thực của ứng dụng của bạn
            return $"{baseUrl}/uploads/orders/{fileName}";
        }

        [HttpPut("use-tickets")]
        //[Authorize(Roles = "Super Admin, Movie Theater Manager Staff")]
        public async Task<IActionResult> GetTicketOrder(string orderCode)
        {
            try
            {
                var order = await _context.Orders.Include(o => o.Tickets).FirstOrDefaultAsync(o => o.OrderCode.Equals(orderCode));
                if (order == null)
                {
                    return NotFound();
                }

                foreach (var item in order.Tickets)
                {
                    item.IsUsed = 1;
                }

                await _context.SaveChangesAsync();

                return Ok();
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

        [HttpPost("SeatReservation/{show_id}")]
        public async Task<IActionResult> CheckSeat(List<int> seat_ids, int show_id)
        {
            try
            {
                // Lặp qua danh sách seat_ids để kiểm tra từng ghế
                foreach (var seat_id in seat_ids)
                {
                    var seatReservation = await _context.SeatReservations.FirstOrDefaultAsync(s => s.SeatId == seat_id && s.ShowId == show_id);

                    if (seatReservation == null)
                    {
                        // Ghế chưa được giữ hoặc giữ chỗ đã hết hạn, thực hiện giữ chỗ mới
                        SeatReservation newSeatReservation = new SeatReservation
                        {
                            SeatId = seat_id,
                            ShowId = show_id,
                            ReservationExpiresAt = null,
                        };

                        _context.SeatReservations.Add(newSeatReservation);
                        await _context.SaveChangesAsync();


                    }
                    else if (seatReservation.ReservationExpiresAt == null)
                    {

                    }
                    else if (DateTime.Now > seatReservation.ReservationExpiresAt)
                    {

                    }
                    else
                    {
                        // Ghế đã được giữ và giữ chỗ vẫn còn hiệu lực, không cần thực hiện gì cả
                        var totalShowss = new
                        {
                            status = false,
                            expiresat = "",
                        };

                        return Ok(totalShowss);
                    }
                }

                var now = DateTime.Now.AddMinutes(5);

                foreach (var seat_id in seat_ids)
                {
                    var seatReservation = await _context.SeatReservations.FirstOrDefaultAsync(s => s.SeatId == seat_id && s.ShowId == show_id);
                    seatReservation.ReservationExpiresAt = now;

                    await _context.SaveChangesAsync();
                }

                await _context.SaveChangesAsync();

                var totalShows = new
                {
                    status = true,
                    expiresat = now,
                };

                return Ok(totalShows);
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
