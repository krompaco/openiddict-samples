using System.Threading.Tasks;

namespace Velusia.Server.Services
{
    public class DummyConsoleEmailSender : IEmailSender
    {
        public Task SendLinkEmailAsync(string emailAddress, string subject, string signInLink)
        {
            // Show in Console /JK
            System.Console.WriteLine(signInLink);

            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }
}
