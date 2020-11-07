using System.Threading.Tasks;

namespace Velusia.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string message);
    }
}
