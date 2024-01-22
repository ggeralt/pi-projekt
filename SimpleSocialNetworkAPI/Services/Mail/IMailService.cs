namespace SimpleSocialNetworkAPI.Services.Mail
{
    public interface IMailService
    {
        Task SendEmail(string toEmail, string subject, string body);
    }
}
