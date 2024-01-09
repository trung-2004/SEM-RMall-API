using RMall.Helper.Email;

namespace RMall.Service.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
    }
}
