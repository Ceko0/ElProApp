namespace ElProApp.Web.Areas.Identity.Pages.Account
{
    using System.Net;
    using System.Net.Mail;
    using Microsoft.AspNetCore.Identity.UI.Services;

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var smtpClient = new SmtpClient(_config["Email:Smtp"], int.Parse(_config["Email:Port"]))
            {
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]),
                EnableSsl = true
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_config["Email:From"], "ElProApp"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(email);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
