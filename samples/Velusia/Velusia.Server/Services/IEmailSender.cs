using System.Threading.Tasks;

namespace Velusia.Server.Services
{
    public interface IEmailSender
    {
        Task SendLinkEmailAsync(string emailAddress, string subject, string signInLink);
    }
}
