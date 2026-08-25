using Services.Models;

namespace Services.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
