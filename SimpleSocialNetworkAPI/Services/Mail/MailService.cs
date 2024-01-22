using MimeKit.Text;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;

namespace SimpleSocialNetworkAPI.Services.Mail
{
    public class MailService : IMailService
    {
        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("driveoff14@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("driveoff14@gmail.com", "cxquykxnrtpxvrya");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
