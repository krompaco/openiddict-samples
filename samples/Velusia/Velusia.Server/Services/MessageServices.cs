﻿using System.Threading.Tasks;

namespace Velusia.Server.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Show in Console /JK
            System.Console.WriteLine(message);

            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }
}
